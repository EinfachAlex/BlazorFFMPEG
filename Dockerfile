FROM ubuntu:20.04 AS ffmpegbuilder
ARG DEBIAN_FRONTEND=noninteractive

RUN apt update
RUN apt install sudo -y

RUN apt install pkg-config -y

#install avahi needed to find ndi sources
RUN apt install avahi-daemon avahi-utils -y

#install packages needed for ffmpeg compile
RUN apt-get update -qq && apt-get -y install \
  autoconf \
  automake \
  build-essential \
  cmake \
  git-core \
  libass-dev \
  libfreetype6-dev \
  libgnutls28-dev \
  libmp3lame-dev \
  libsdl2-dev \
  libtool \
  libva-dev \
  libvdpau-dev \
  libvorbis-dev \
  libxcb1-dev \
  libxcb-shm0-dev \
  libxcb-xfixes0-dev \
  meson \
  ninja-build \
  pkg-config \
  texinfo \
  wget \
  yasm \
  zlib1g-dev \
  xxd \
  vim

#download nvidia nvenc files
RUN mkdir -p ~/ffmpeg_sources ~/bin
RUN cd ~/ffmpeg_sources && git clone https://git.videolan.org/git/ffmpeg/nv-codec-headers.git && cd nv-codec-headers && sudo make install

#install ffmpeg codecs
RUN apt-get install nasm -y

RUN apt-get install libx264-dev -y

RUN apt-get install libx265-dev libnuma-dev -y

RUN apt-get install libfdk-aac-dev -y

RUN apt-get install libunistring-dev -y

#install svtav1 codec
RUN cd ~/ffmpeg_sources && \
git -C SVT-AV1 pull 2> /dev/null || git clone https://gitlab.com/AOMediaCodec/SVT-AV1.git && \
mkdir -p SVT-AV1/build && \
cd SVT-AV1/build && \
PATH="$HOME/bin:$PATH" cmake -G "Unix Makefiles" -DCMAKE_INSTALL_PREFIX="$HOME/ffmpeg_build" -DCMAKE_BUILD_TYPE=Release -DBUILD_DEC=OFF -DBUILD_SHARED_LIBS=OFF .. && \
PATH="$HOME/bin:$PATH" make -j 24 && \
make install

#install vmaf
RUN cd ~/ffmpeg_sources && \
wget https://github.com/Netflix/vmaf/archive/v2.3.1.tar.gz && \
tar xvf v2.3.1.tar.gz && \
mkdir -p vmaf-2.3.1/libvmaf/build &&\
cd vmaf-2.3.1/libvmaf/build && \
meson setup -Denable_tests=false -Denable_docs=false --buildtype=release --default-library=static .. --prefix "$HOME/ffmpeg_build" --bindir="$HOME/ffmpeg_build/bin" --libdir="$HOME/ffmpeg_build/lib" && \
ninja -j 24 && \
ninja install

#fix error packages not found
RUN apt-get install libunistring-dev -y

RUN apt-get install --no-install-recommends\
    ninja-build \
    python3 \
    python3-pip \
    python3-setuptools \
    python3-wheel \
    ninja-build \
    wget \
    doxygen \
    autoconf \
    automake \
    cmake \
    g++ \
    gcc \
    pkg-config \
    make \
    nasm \
    yasm -y

RUN pip3 install --user meson

#install av1decoder
RUN cd ~/ffmpeg_sources && \
git -C dav1d pull 2> /dev/null || git clone --depth 1 https://code.videolan.org/videolan/dav1d.git && \
mkdir -p dav1d/build && \
cd dav1d/build && \
meson setup -Denable_tools=false -Denable_tests=false --default-library=static .. --prefix "$HOME/ffmpeg_build" --libdir="$HOME/ffmpeg_build/lib" && \
ninja && \
ninja install

#download and compile ffmpeg
RUN cd ~/ffmpeg_sources && wget https://downloads.ndi.tv/SDK/NDI_SDK_Linux/Install_NDI_SDK_v5_Linux.tar.gz && \
 tar -xvf Install_NDI_SDK_v5_Linux.tar.gz && echo "y" | ./Install_NDI_SDK_v5_Linux.sh

RUN cd ~/ffmpeg_sources && mkdir ndi && mv "./NDI SDK for Linux/"* ndi/ && ls ndi/ && cp ndi/lib/* /lib -r && cp ndi/include/* /usr/include -r

RUN cd ~/ffmpeg_sources && \
#wget -O ffmpeg-snapshot.tar.bz2 https://ffmpeg.org/releases/ffmpeg-5.1.tar.gz && \
#tar -xvf ffmpeg-snapshot.tar.bz2 && \
#mv ffmpeg-5.1/ ffmpeg/ && \
git clone https://github.com/FFmpeg/FFmpeg && \
git clone https://framagit.org/tytan652/ffmpeg-ndi-patch.git

RUN cd ~/ffmpeg_sources/FFmpeg && \
#git checkout release/5.1 && \
git config user.name "alexander" && git config user.email "einfachalex@sags-per-mail.de" && \
git am ../ffmpeg-ndi-patch/master_Revert-lavd-Remove-libndi_newtek.patch && \
cp ../ffmpeg-ndi-patch/libavdevice/libndi_newtek_* libavdevice/ && \
PATH="$HOME/bin:$PATH" PKG_CONFIG_PATH="$HOME/ffmpeg_build/lib/pkgconfig" ./configure \
  --extra-cflags="-I/root/ffmpeg_sources/ndi/include" \
  --extra-ldflags="-L/root/ffmpeg_sources/ndi/lib" \
  --enable-libndi_newtek \
  --enable-nonfree \
  --enable-gpl \
  --enable-libx264 \
  --enable-libx265 \
  --enable-libsvtav1 \
  --enable-nvenc \
  --enable-libvmaf \
  --ld="g++" \
  --pkg-config-flags="--static" \
  --enable-static \
  --extra-cflags="-I/usr/local/include -I/usr/lib/include" \
  --extra-cxxflags="-I/usr/local/include -I/usr/lib/include" \
  --extra-ldflags="-L/usr/local/lib -L/usr/local/lib64 -L/usr/lib -L/usr/lib64" \
  --extra-libs=-pthread \
  --extra-libs=-lm \
 --enable-libvmaf \
 --enable-libdav1d && \
PATH="$HOME/bin:$PATH" make -j 24

RUN cd ~/ffmpeg_sources/FFmpeg && make install && \
hash -r

#show encoders
RUN ffmpeg -encoders

RUN which ffmpeg

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["BlazorFFMPEG.Backend/BlazorFFMPEG.Backend.csproj", "BlazorFFMPEG.Backend/"]
COPY ["BlazorFFMPEG/BlazorFFMPEG.csproj", "BlazorFFMPEG/"]
COPY ["BlazorFFMPEG.Shared/BlazorFFMPEG.Shared.csproj", "BlazorFFMPEG.Shared/"]
COPY ["EinfachAlexUtils/EinfachAlex.Utils.csproj", "EinfachAlexUtils/"]
RUN dotnet restore "BlazorFFMPEG.Backend/BlazorFFMPEG.Backend.csproj"
COPY . .
WORKDIR "/src/BlazorFFMPEG.Backend"
RUN dotnet build "BlazorFFMPEG.Backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BlazorFFMPEG.Backend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=ffmpegbuilder /usr/local/bin/ffmpeg /usr/local/bin/ffmpeg
ENTRYPOINT ["dotnet", "BlazorFFMPEG.Backend.dll"]

