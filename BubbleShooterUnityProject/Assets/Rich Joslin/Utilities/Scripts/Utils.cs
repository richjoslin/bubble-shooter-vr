using UnityEngine;

namespace RichJoslin
{
	public class Utils
	{
		public static float RoundAwayFromZero(float n)
		{
			if (n == 0f) return 0f;
			return Mathf.Ceil(Mathf.Abs(n)) * Mathf.Sign(n);
		}

		public static bool IsEven(int n)
		{
			return n % 2 == 0;
		}

		public static bool IsOdd(int n)
		{
			return !IsEven(n);
		}

		public static string PadZeroes(int value, int totalLength)
		{
			string outString = value.ToString();
			outString = string.Format("0000000000000000000000000{0}", outString);
			return outString.Substring(outString.Length - totalLength);
		}

		public static Color32 ColorFromHTML(string hexString, byte a = 255)
		{
			if (hexString.Length != 7) return Color.gray;
			char[] hexChars = hexString.ToCharArray();
			byte r = (byte)(HexToInt(hexChars[1]) * 16 + HexToInt(hexChars[2]));
			byte g = (byte)(HexToInt(hexChars[3]) * 16 + HexToInt(hexChars[4]));
			byte b = (byte)(HexToInt(hexChars[5]) * 16 + HexToInt(hexChars[6]));
			return new Color32(r, g, b, a);
		}

		public static int HexToInt(char hex)
		{
			switch (hex)
			{
			case '0': return 0;
			case '1': return 1;
			case '2': return 2;
			case '3': return 3;
			case '4': return 4;
			case '5': return 5;
			case '6': return 6;
			case '7': return 7;
			case '8': return 8;
			case '9': return 9;
			case 'A': return 10;
			case 'B': return 11;
			case 'C': return 12;
			case 'D': return 13;
			case 'E': return 14;
			case 'F': return 15;
			case 'a': return 10;
			case 'b': return 11;
			case 'c': return 12;
			case 'd': return 13;
			case 'e': return 14;
			case 'f': return 15;
			}
			return 15;
		}
	}
}
