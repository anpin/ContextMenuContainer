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
  in 
 mkShell {
  packages = [
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
  DOTNET_ROOT = "${sdk}";
  ANDROID_HOME = "${android-sdk}/share/android-sdk";
  ANDROID_SDK_ROOT = "${android-sdk}/share/android-sdk";
  JAVA_HOME = jdk17.home;
}
