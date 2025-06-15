{ pkgs ? import <nixpkgs> { }, android-nixpkgs ? import <android-nixpkgs> { } }:
with pkgs;
let 
  sdk =   dotnetCorePackages.dotnet_9.sdk; 
  
  android-sdk =  android-nixpkgs.sdk.${system} (sdkPkgs: with sdkPkgs; [
            
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
fhs = pkgs.buildFHSEnv {
  name = "dotnet-android-env";
  targetPkgs = pkgs: [
    sdk
    tree
    android-sdk
    gradle
    jdk17
    aapt
    llvm_18
    zip
    nuget-to-nix
    nixpkgs-fmt
    nil
    jetbrains.rider
    nodejs
    act
    
  ];
#  DOTNET_ROOT = "${sdk}";
#  ANDROID_HOME = "${android-sdk}/share/android-sdk";
#  ANDROID_SDK_ROOT = "${android-sdk}/share/android-sdk";
#  JAVA_HOME = jdk17.home;

  profile = ''
    export DOTNET_ROOT="${sdk}"
    export ANDROID_HOME="${android-sdk}/share/android-sdk"
    export ANDROID_SDK_ROOT="${android-sdk}/share/android-sdk"
    export JAVA_HOME="${pkgs.jdk17.home}"
    mkdir -p ~/.npm
    npm config set prefix ~/.npm
    export PATH="$HOME/.npm/bin:$PATH"
    npm install -g appium
    if ! appium driver list --installed --json | jq -e '.drivers | has("uiautomator2")' >/dev/null; then
      appium driver install uiautomator2
    fi    
  '';

  runScript = "bash --init-file /etc/profile";
#  runScript = "bash";
}; in fhs.env
