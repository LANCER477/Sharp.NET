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

            string characterSet = "";
            switch (mode)
            {
                case OtpMode.Digits:
                    characterSet = DigitsChars;
                    break;
                case OtpMode.Letters:
                    characterSet = LettersChars;
                    break;
                case OtpMode.Mixed:
                    characterSet = MixedChars;
                    break;
                default:
                    throw new ArgumentException("Невідомий режим ОТР");
            }

            char[] otp = new char[length];
            for (int i = 0; i < length; i++)
            {
                otp[i] = characterSet[_random.Next(characterSet.Length)];
            }

            return new string(otp);
        }
    }
}
