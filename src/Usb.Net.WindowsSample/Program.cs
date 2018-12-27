﻿using Device.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Usb.Net.Windows;

namespace Usb.Net.WindowsSample
{
    class Program
    {
        static void Main(string[] args)
        {
            Go();
            Console.ReadLine();
        }

        private async static Task Go()
        {
            //Register the factory for creating Usb devices. This only needs to be done once.
            WindowsUsbDeviceFactory.Register();

            //Set the factory to the Trezor Win USB guid
            ((WindowsUsbDeviceFactory)DeviceManager.Current.DeviceFactories.First(f => f is WindowsUsbDeviceFactory)).ClassGuid = new Guid("0263b512-88cb-4136-9613-5c8e109d8ef5");

            //Note: other custom device types could be added here

            //Define the types of devices to search for. This particular device can be connected to via USB, or Hid
            var deviceDefinitions = new List<DeviceDefinition>
            {
                new DeviceDefinition{ DeviceType= DeviceType.Hid, VendorId= 0x534C, ProductId=0x0001, Label="Trezor One Firmware 1.6.x" },
                new DeviceDefinition{ DeviceType= DeviceType.Usb, VendorId= 0x1209, ProductId=0x53C1, ReadBufferSize=64, WriteBufferSize=64, Label="Trezor One Firmware 1.7.x" },
                new DeviceDefinition{ DeviceType= DeviceType.Usb, VendorId= 0x1209, ProductId=0x53C0, ReadBufferSize=64, WriteBufferSize=64, Label="Model T" }
            };

            //Get the first available device and connect to it
            var devices = await DeviceManager.Current.GetDevices(deviceDefinitions);
            var trezorDevice = devices.FirstOrDefault();
            await trezorDevice.InitializeAsync();

            //Create a buffer with 3 bytes (initialize)
            var buffer = new byte[64];
            buffer[0] = 0x3f;
            buffer[1] = 0x23;
            buffer[2] = 0x23;

            //Write the data to the device
            await trezorDevice.WriteAsync(buffer);

            //Read the response
            var readBuffer = await trezorDevice.ReadAsync();

        }
    }
}
