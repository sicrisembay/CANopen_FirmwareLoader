# CANopen Firmware Loader
This repo contains two PC applications which are written in C#.  As of now, only Peak PCAN-USB is supported.

## Firmware Loader
This is an application to load a firmware image in Intel hex format to a CANopen node with bootloader.  I mainly
use this application to test my bootloader project ([CANopenNode_C2000_bootloader](https://github.com/sicrisembay/CANopenNode_C2000_bootloader)).

## LSS Manager
This application is a LSS Manager that is used to change node ID and bit rate.