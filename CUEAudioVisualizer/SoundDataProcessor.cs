using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CUEAudioVisualizer.Exceptions;
using Un4seen.Bass;
using Un4seen.BassWasapi;

namespace CUEAudioVisualizer
{
    //This class was written a long time ago as a newbie programmer, there are tons of magic values, suboptimal usage of variables, hardcoded constants, etc etc
    //I apologise for this mess of code but it works decently well so i'm not going to be fixing it
    class SoundDataProcessor
    {
        public const int BarCount = 1000;

        public float[] BarValues = new float[BarCount];
        public float VolumeScalar = 1f;
        public float SmoothingScalar = 0f;
        public float SongBeat = 0;
        public float AveragedVolume = 0;
        public float ImmediateVolume = 0;
        public int WASAPIDeviceIndex = -1;

        private BASSData maxFFT = (BASSData.BASS_DATA_FFT8192);
        private float[] fftData = new float[4096];
        private float[] freqVolScalar = new float[BarCount];
        private float[] barValues = new float[BarCount];
        private float[] lastBarValues = new float[BarCount];
        private float lastBeat = 0f;
        private int sampleFrequency = 48000;
        private int MaximumFrequency = 21000;
        private int MinimumFrequency = 0;
        private int maximumFrequencyIndex;
        private int minimumFrequencyIndex;
        private int deviceNumber;
        private int[] logIndex = new int[BarCount];
        private bool deviceInitialized = false;
        private WASAPIPROC WasapiProc;

        public SoundDataProcessor(int deviceIndex)
        {
            BuildLookupTables();
            WASAPIDeviceIndex = deviceIndex;
            WasapiProc = new WASAPIPROC(IgnoreDataProc);
        }

        //Calculates Average Volume, Immediate Volume, and Bar Data from the currently active WASAPI device
        public void Process()
        {
            if (deviceNumber != WASAPIDeviceIndex || !deviceInitialized)
            {
                UpdateDevice();
                deviceNumber = BassWasapi.BASS_WASAPI_GetDevice();
            }
            if (!deviceInitialized) return;

            GetBarData();
            float CurVol = BassWasapi.BASS_WASAPI_GetDeviceLevel(deviceNumber, -1) * VolumeScalar;
            ImmediateVolume = CurVol;
            AveragedVolume = Utility.Clamp(Utility.LinearInterpolate(AveragedVolume, CurVol, 0.02f), 0f, 1f);
        }

        //Reinitializes active WASAPI device if necessary
        private void UpdateDevice()
        {
            if (WASAPIDeviceIndex == -1) return;
            if (deviceInitialized)
            {
                Console.WriteLine("Deinitializing WASAPI device");
                BassWasapi.BASS_WASAPI_Stop(true);
                BassWasapi.BASS_WASAPI_Free();
                deviceInitialized = false;
            }
            BASS_WASAPI_DEVICEINFO devInfo = BassWasapi.BASS_WASAPI_GetDeviceInfo(WASAPIDeviceIndex);
            if (devInfo == null)
            {
                throw new WASAPIInitializationException("Device " + WASAPIDeviceIndex + " is invalid!");
            }
            if (!BassWasapi.BASS_WASAPI_Init(WASAPIDeviceIndex, devInfo.mixfreq, 2, BASSWASAPIInit.BASS_WASAPI_AUTOFORMAT | BASSWASAPIInit.BASS_WASAPI_BUFFER, 0f, 0f, WasapiProc, IntPtr.Zero))
            {
                BASSError error = Bass.BASS_ErrorGetCode();
                throw new WASAPIInitializationException("Unable to initialize WASAPI device " + WASAPIDeviceIndex, error);
            }
            if (!BassWasapi.BASS_WASAPI_Start())
            {
                BASSError error = Bass.BASS_ErrorGetCode();
                throw new WASAPIInitializationException("Unable to start WASAPI!", error);
            }
            Console.WriteLine("WASAPI device initialized");
            deviceNumber = WASAPIDeviceIndex;
            sampleFrequency = devInfo.mixfreq;
            BuildLookupTables();
            deviceInitialized = true;
        }

        private void BuildLookupTables()
        {
            maximumFrequencyIndex = Math.Min(Utils.FFTFrequency2Index(MaximumFrequency, 8192, sampleFrequency) + 1, 4095);
            minimumFrequencyIndex = Math.Min(Utils.FFTFrequency2Index(MinimumFrequency, 8192, sampleFrequency), 4095);
            freqVolScalar[0] = 1f;
            for (int i = 1; i < BarCount; i++)
            {
                logIndex[i] = (int)((Math.Log(BarCount, BarCount) - Math.Log(BarCount - i, BarCount)) * (maximumFrequencyIndex - minimumFrequencyIndex) + minimumFrequencyIndex);
                freqVolScalar[i] = 1 + (float)Math.Sqrt((double)i / (double)BarCount) * 1.25f;
            }
        }

        private void GetBarData()
        {
            //Get FFT data
            BassWasapi.BASS_WASAPI_GetData(fftData, (int)maxFFT);
            int barIndex = 0;
            //Calculate bar values by squaring fftData from log(x) fft bin, and multiply by a few magic values to end up with a somewhat reasonable graphical representation of the sound
            for (barIndex = 0; barIndex < BarCount; barIndex++)
            {
                barValues[barIndex] = ((float)Math.Sqrt(fftData[logIndex[barIndex]]) * 15f * VolumeScalar) * freqVolScalar[barIndex];
            }
            barIndex = 0;

            //This chunk of code is supposed to do a rolling average to smooth out the lower values to look cleaner for another visualizer i'm working on
            float preScaled;

            float preScaled1 = barValues[barIndex];
            preScaled1 += barValues[barIndex + 1];
            preScaled1 /= 2f;
            BarValues[barIndex] = Utility.Clamp(preScaled1, 0f, 1f);
            BarValues[barIndex] = Utility.LinearInterpolate(BarValues[barIndex], lastBarValues[barIndex], SmoothingScalar); //man this is nasty
            lastBarValues[barIndex] = BarValues[barIndex];

            barIndex++;

            preScaled1 = barValues[barIndex - 1] * 0.75f;
            preScaled1 += barValues[barIndex];
            preScaled1 += barValues[barIndex + 1] * 0.75f;
            preScaled1 /= 2.5f;
            BarValues[barIndex] = Utility.Clamp(preScaled1, 0f, 1f);
            BarValues[barIndex] = Utility.LinearInterpolate(BarValues[barIndex], lastBarValues[barIndex], SmoothingScalar);
            lastBarValues[barIndex] = BarValues[barIndex];

            for (barIndex = 2; barIndex < 50; barIndex++)
            {
                preScaled = barValues[barIndex - 2] * 0.5f;
                preScaled += barValues[barIndex - 1] * 0.75f;
                preScaled += barValues[barIndex];
                preScaled += barValues[barIndex + 1] * 0.75f;
                preScaled += barValues[barIndex + 2] * 0.5f;
                preScaled /= 3.5f;
                BarValues[barIndex] = Utility.Clamp(preScaled, 0f, 1f);
                BarValues[barIndex] = Utility.LinearInterpolate(BarValues[barIndex], lastBarValues[barIndex], SmoothingScalar);
                lastBarValues[barIndex] = BarValues[barIndex];
            }
            for (barIndex = 50; barIndex < 999; barIndex++)
            {
                preScaled = barValues[barIndex - 1] * 0.75f;
                preScaled += barValues[barIndex];
                preScaled += barValues[barIndex + 1] * 0.75f;
                preScaled /= 2.5f;
                BarValues[barIndex] = Utility.Clamp(preScaled, 0f, 1f);
                BarValues[barIndex] = Utility.LinearInterpolate(BarValues[barIndex], lastBarValues[barIndex], SmoothingScalar);
                lastBarValues[barIndex] = BarValues[barIndex];
            }
            preScaled = barValues[barIndex - 1];
            preScaled += barValues[barIndex];
            preScaled /= 2f;
            BarValues[barIndex] = Utility.Clamp(preScaled, 0f, 1f);
            BarValues[barIndex] = Utility.LinearInterpolate(BarValues[barIndex], lastBarValues[barIndex], SmoothingScalar);
            lastBarValues[barIndex] = BarValues[barIndex];

            //Calculate the song beat
            float Sum = 0f;
            for (int i = 2; i < 28; i++)
            {
                Sum += (float)Math.Sqrt(barValues[i]); //Prettier scaling > Accurate scaling
            }
            SongBeat = (Sum / 25f);
            SongBeat = Utility.LinearInterpolate(SongBeat, lastBeat, SmoothingScalar);
            lastBeat = SongBeat;
        }

        

        private static int IgnoreDataProc(IntPtr buffer, int length, IntPtr user)
        {
            return 1;
        }
    }
}
