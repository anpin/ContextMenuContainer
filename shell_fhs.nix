
{ pkgs ? import <nixpkgs> { }
, android-nixpkgs ? import <android-nixpkgs> { }
, nixpkgs ? import <nixpkgs> { }
}:

with pkgs;
let
  android-sdk = android-nixpkgs.sdk.${system} (sdkPkgs: with sdkPkgs; [
    build-tools-35-0-0
    build-tools-34-0-0
    build-tools-30-0-0
    cmdline-tools-latest
    emulator
    platform-tools
    platforms-android-35
    platforms-android-34
    platforms-android-30
  ]);

  # FHS environment for installing official .NET SDK
  dotnet-fhs = buildFHSEnv {
    name = "dotnet-fhs";
    targetPkgs = pkgs: (with pkgs; [
      coreutils
      util-linux
      gawk
      gnused
      gnugrep
      which
      git
      curl
      wget
      unzip
      procps
      
      icu
      zlib
      openssl
      
      gcc
      gcc-unwrapped
      glibc
      glibc.dev
      
      stdenv.cc.cc.lib
      llvm_18
      glibc
      skia
      libGL
      libGLU
      xorg.libX11
      xorg.libXi
      zlib
      openssl
      xorg.libICE
      xorg.libSM
      fontconfig
      gtk3
      
      tree
      android-sdk
      gradle
      jdk17
      aapt
      zip
      nuget-to-json
      nixpkgs-fmt
      nil
      python3
      jetbrains.rider
      nodejs
      windsurf.fhs
      
    ]);
    
    profile = ''
      # Environment variables
      export ANDROID_HOME=${android-sdk}/share/android-sdk
      export ANDROID_SDK_ROOT=${android-sdk}/share/android-sdk
      export JAVA_HOME=${jdk17.home}
      export DOTNET_ROOT=~/.dotnet
      export PATH="$DOTNET_ROOT:$PATH"
      # Information on how to install .NET SDK
      echo ""
      echo "====================================================================="
      echo "FHS Environment for .NET SDK Installation"
      echo "====================================================================="
      echo ""
      echo "To install the official .NET SDK, run:"
      echo ""
      echo "  curl -sSL https://dot.net/v1/dotnet-install.sh | bash -s -- --channel 9.0"
      echo "  # Add more channels as needed, for example:"
      echo "  curl -sSL https://dot.net/v1/dotnet-install.sh | bash -s -- --channel 8.0"
      echo ""
      echo "The SDK will be installed to ~/.dotnet"
      echo "Add it to your PATH within this shell:"
      echo ""
      echo "  export PATH=~/.dotnet:$PATH"
      echo ""
      echo "====================================================================="
      echo ""
    '';
            runScript = "bash --init-file /etc/profile";

  };

in dotnet-fhs.env
