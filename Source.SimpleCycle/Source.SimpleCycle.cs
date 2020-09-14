﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SimpleLed;

namespace Source.SimpleCycle
{
    public class SimpleRGBCycleDriver : ISimpleLed
    {

        public T GetConfig<T>() where T : SLSConfigData
        {
            //TODO throw new NotImplementedException();
            return null;
        }

        public void PutConfig<T>(T config) where T : SLSConfigData
        {
            //TODO throw new NotImplementedException();
        }
        public string Name()
        {
            return "Simple Cycle";
        }

        private const int LEDCount = 32;
        private Timer timer;
        private int currentPos = 0;
        ControlDevice.LedUnit[] leds = new ControlDevice.LedUnit[32];
        ControlDevice.LedUnit[] fanLeds = new ControlDevice.LedUnit[32];
        ControlDevice.LedUnit[] fanLeds2 = new ControlDevice.LedUnit[32];
        private int dly = 0;

        private SimpleRGBCycleLEDData rgbPropData = new SimpleRGBCycleLEDData
        {
            RInc = 0.005f,
            GInc = 0.0003f,
            BInc = 0.0001f,
            R = 0,
            G = 0,
            B = 0
        };
        public SimpleRGBCycleDriver()
        {
            for (int i = 0; i < LEDCount; i++)
            {
                leds[i] = new ControlDevice.LedUnit
                {
                    LEDName = "LED " + i,
                    Data = new SimpleRGBCycleLEDData
                    {
                        LEDNumber = i,
                        R = i * 0.01f,
                        G = i * 0.004f,
                        B = i * .003f
                    },
                };

                fanLeds[i] = new ControlDevice.LedUnit
                {
                    LEDName = "LED " + i,
                    Data = new SimpleRGBCycleLEDData
                    {
                        LEDNumber = i,
                        R = i * 0.01f,
                        G = i * 0.004f,
                        B = i * .003f
                    },
                };

                fanLeds2[i] = new ControlDevice.LedUnit
                {
                    LEDName = "LED " + i,
                    Data = new SimpleRGBCycleLEDData
                    {
                        LEDNumber = i,
                    },
                };
            }

            timer = new Timer(TimerCallback, null, 0, 33);
        }

        private void TimerCallback(object state)
        {
            for (int i = 0; i < LEDCount; i++)
            {
                var tmp = ((SimpleRGBCycleLEDData)leds[i].Data);
                if (tmp.R + tmp.RInc > 1f || tmp.R + tmp.RInc < 0f) tmp.RInc = -tmp.RInc;
                if (tmp.G + tmp.GInc > 1f || tmp.G + tmp.GInc < 0f) tmp.GInc = -tmp.RInc;
                if (tmp.B + tmp.BInc > 1f || tmp.B + tmp.BInc < 0f) tmp.BInc = -tmp.RInc;

                tmp.R = tmp.R + tmp.RInc;
                tmp.G = tmp.G + tmp.GInc;
                tmp.B = tmp.B + tmp.BInc;

                leds[i].Color = new LEDColor((int)(tmp.R * 255), (int)(tmp.G * 255), (int)(tmp.B * 255));
            }

            for (int i = 0; i < LEDCount; i++)
            {
                var tmp = ((SimpleRGBCycleLEDData)fanLeds[i].Data);

                LEDColor cl = ((currentPos + i) % (LEDCount / 2)) != 0 ? new LEDColor(0, 0, 0) : new LEDColor(255, 0, 255);

                fanLeds[i].Color = cl;
            }

            dly = dly + 1;

            if (rgbPropData.R + rgbPropData.RInc > 1f || rgbPropData.R + rgbPropData.RInc < 0f)
                rgbPropData.RInc = -rgbPropData.RInc;
            if (rgbPropData.G + rgbPropData.GInc > 1f || rgbPropData.G + rgbPropData.GInc < 0f)
                rgbPropData.GInc = -rgbPropData.RInc;
            if (rgbPropData.B + rgbPropData.BInc > 1f || rgbPropData.B + rgbPropData.BInc < 0f)
                rgbPropData.BInc = -rgbPropData.RInc;

            rgbPropData.R = rgbPropData.R + rgbPropData.RInc;
            rgbPropData.G = rgbPropData.G + rgbPropData.GInc;
            rgbPropData.B = rgbPropData.B + rgbPropData.BInc;


            var lcolv = new LEDColor((int)(rgbPropData.R * 255), (int)(rgbPropData.G * 255), (int)(rgbPropData.B * 255));

            for (int i = 0; i < LEDCount; i++)
            {
                var tmp = ((SimpleRGBCycleLEDData)fanLeds2[i].Data);

                LEDColor cl = ((currentPos + i) % (LEDCount / 2)) != 0 ? new LEDColor(0, 0, 0) : lcolv;

                fanLeds2[i].Color = cl;
            }

            currentPos++;
        }



        public void Configure(DriverDetails driverDetails)
        {

        }

        public List<ControlDevice> GetDevices()
        {

            Bitmap prop;
            Bitmap rgb;

            Assembly myAssembly = Assembly.GetExecutingAssembly();
            using (Stream myStream = myAssembly.GetManifestResourceStream("Source.SimpleCycle.prop.png"))
            {
                prop = (Bitmap)Image.FromStream(myStream);
            }

            using (Stream myStream = myAssembly.GetManifestResourceStream("Source.SimpleCycle.rgbcycle.png"))
            {
                rgb = (Bitmap)Image.FromStream(myStream);
            }


            return new List<ControlDevice>
            {
                new ControlDevice
                {
                    Name = "RGB Cycler",
                    Driver = this,
                    LEDs = leds,
                    DeviceType = DeviceTypes.Effect,
                    ProductImage = rgb
                },
                new ControlDevice
                {
                    Name = "Purple Propella",
                    Driver = this,
                    LEDs = fanLeds,
                    DeviceType = DeviceTypes.Effect,
                    ProductImage = prop
                },
                new ControlDevice
                {
                    Name = "RGB Propella",
                    Driver = this,
                    LEDs = fanLeds2,
                    DeviceType = DeviceTypes.Effect,
                    ProductImage = prop
                }
            };

        }

        public void Push(ControlDevice controlDevice)
        {

        }

        public void Pull(ControlDevice controlDevice)
        {

        }

        public DriverProperties GetProperties()
        {
            return new DriverProperties
            {
                SupportsPull = false,
                SupportsPush = false,
                IsSource = true,
                Id = Guid.Parse("a9440d02-8ca3-4e35-a9a3-88b024cc0e2d"),
                Author = "mad ninja",
                Blurb = "Some simple fun effects",
                CurrentVersion = new ReleaseNumber(1, 2, 0, 1),
                GitHubLink = "https://github.com/SimpleLed/Source.SimpleCycle",
                IsPublicRelease = false,
                SupportsCustomConfig = false
            };
        }

        public class SimpleRGBCycleLEDData : ControlDevice.LEDData
        {

            public float R { get; set; }
            public float G { get; set; }
            public float B { get; set; }
            public float BInc = 0.1f;
            public float GInc = 0.04f;
            public float RInc = 0.01f;
        }

        public void Dispose()
        {
            timer.Dispose();
        }
    }
}
