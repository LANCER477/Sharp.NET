using System;
using System.Collections.Generic;
using System.Text;

namespace SharpKnP321.Users
{
    public enum OtpMode
    {
        Digits, 
        Letters, 
        Mixed 
    }

    public class OtpService
    {
        private readonly Random _random = new Random();

        
        private const string DigitsChars = "0123456789";
        private const string LettersChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

       
        private const string MixedChars = "abcdefghijkmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ23456789";

        public string Generate(int length, OtpMode mode)
        {
            if (length <= 0)
            {
                throw new ArgumentException("Довжина паролю має бути більшою за нуль", nameof(length));
            }

            string characterSet = mode switch
            {
                OtpMode.Digits => DigitsChars,
                OtpMode.Letters => LettersChars,
                OtpMode.Mixed => MixedChars,
                _ => throw new ArgumentOutOfRangeException(nameof(mode), "Невідомий режим ОТР")
            };

            char[] otp = new char[length];
            for (int i = 0; i < length; i++)
            {
                otp[i] = characterSet[_random.Next(characterSet.Length)];
            }

            return new string(otp);
        }
    }
}
