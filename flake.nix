{
  description = "basic dotnet MAUI shell";
  
  inputs = {

    nixpkgs = {
      url = "github:nixos/nixpkgs?ref=nixos-unstable";
    };

    android-nixpkgs = {
      url = "github:tadfisher/android-nixpkgs";
    };
  };

  outputs = inputs@{ self, nixpkgs, android-nixpkgs, ... }:
    let 
    system = "x86_64-linux";
    pkgs = import nixpkgs { 
      inherit system ;
      config = {
        allowUnfree = true;
       };
      };
    inherit (inputs.nixpkgs) lib; 
    in {
      
      devShells.${system} = { 
        default = import ./shell.nix {inherit pkgs android-nixpkgs;};
        fhs = import ./shell_fhs.nix {inherit pkgs android-nixpkgs;};
      };

  };
}
