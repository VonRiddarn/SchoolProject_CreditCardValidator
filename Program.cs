using System;
using System.Globalization;

internal class Program
{
	// To test the program, use: https://www.paypalobjects.com/en_GB/vhelp/paypalmanager_help/credit_card_numbers.htm
	
	/*Why we create a list of manifacturers:
	We need to manually create a new global scope list of card manifacturers so we can compare the cardnumber with these.
	A manifacturers constructor takes 3 values:
	The manifacturer name
	How many starting digits on the card this manifacturer use to represent themselves
	What starting digits (definitions) the manifacturer use to represent themselves*/
	
	static CardManifacturer[] manifacturers = new CardManifacturer[]
	{
		new CardManifacturer("VISA", 1, new string[]{"4"}),
		new CardManifacturer("MASTERCARD", 2, new string[]{"51", "52", "53", "54","55"}),
		new CardManifacturer("AMERICAN EXPRESS", 2, new string[]{"34", "37"}),
		new CardManifacturer("DISCOVER", 1, new string[]{"6"}),
		new CardManifacturer("DINERS CLUB", 2, new string[]{"30", "38"}),
		new CardManifacturer("JCB", 2, new string[]{"35"}),
		new CardManifacturer("AUSTRAILIAN BANKCARD", 2, new string[] {"56"})
	};

	// Program starts here.
	public static void Main(string[] args)
	{
		// Program mainframe loop - can only end by termination.
		while (true)
		{
			Console.Clear();
			Console.WriteLine("Press 'ctrl + c' to exit.");
			Console.WriteLine("Type in a card number.");

			string cardNumber = Console.ReadLine();

			//Console.Clear();

			// Use the CheckSumValidation methods return to see if the card is valid.
			if (CheckSumValidation(cardNumber))
			{
				string ending = cardNumber.Substring(cardNumber.Length - 4, 4);
				Console.WriteLine("--------------------");
				Console.WriteLine("CARD ACCEPTED");
				Console.WriteLine($"{GetManifacturerName(cardNumber)} ending in {ending}.");
				Console.WriteLine("--------------------");
			}
			else
			{
				Console.WriteLine("--------------------");
				Console.WriteLine("ERROR: Not a valid card number!");
				Console.WriteLine("--------------------");
			}

			Console.WriteLine("Press any key to continue...");
			Console.ReadKey();
		}
	}

	/*Explanation GetManifacturerName:
	Compare the cardnumber with the global scope manifacturers list.
	If the card matches 1 maifacturer, return that manifacturer namne.
	If the card matches 2 or more manifacturers, return UNKNOWN.
	If the card matches no manifacturers, return UNKNOWN.*/
	
	///<summary>Return the manifacturer name of the inputed card number.</summary>
	///<remarks>Default to "UNKNOWN"</remarks>
	static string GetManifacturerName(string cardNumber)
	{
		// Cache a null value to see if we have discovered a manifacturer yet.
		CardManifacturer currentManifacturer = null;

		foreach (CardManifacturer manifacturer in manifacturers)
		{
			// Cut out ONLY the amount of numbers that define this card from the inputed card.
			// EG MasterCard : The first 2 numbers | say 55.
			string cardDefinition = cardNumber.Substring(0, manifacturer.numbersToCheck);

			// Loop through all the definitions of this card in the global scope cards list.
			// Eg MasterCard: 51, 52, 53, 54, 55
			foreach (string definition in manifacturer.definitions)
			{
				if (cardDefinition == definition)
				{
					// If the inputed card matches, and we haven't matched with another manifacturer:
					// Cache this manifacturer
					if (currentManifacturer == null)
						currentManifacturer = manifacturer;

					// If the inputed card matches and we already have another manifacturer match:
					// Return UNKNOWN as we can't tell which manifacturer is correct.
					else
						return "UNKNOWN";
				}
			}
		}
		
		// If we have no match for the manifacturer after going through all manidacturers:
		// Return UNKNOWN.
		if (currentManifacturer == null)
			return "UNKNOWN";
			
		// If we have a match for the manifacturer, return that manifacturer name.
		return currentManifacturer.name;
	}


	/*Why we send cardnumber as string: 
	We feed the card as string as the max int value is 2,147,483,647 (10 digits)
	Using a string makes it easier to parse and modify anyway, so it makes the most sense.*/
	
	/// <summary>Check if the inputed string is a valid card number.</summary>
	static bool CheckSumValidation(string cardNumber)
	{

		int[] parsedCardNumber = new int[cardNumber.Length];

		// TODO: GÖr till en metod senare
		for (int i = 0; i < cardNumber.Length; i++)
		{
			if (int.TryParse(cardNumber[i].ToString(), out int n))
				parsedCardNumber[i] = n;
			else
			{
				// Since the card has invalid input, the checksum will automatically be invalidated.
				Console.WriteLine("ERROR: Card number may only contain whole numbers.");
				return false;
			}
		}

		// Do we have an even or odd number of numbers on our card?
		// We need to separate every other number on the card into one array, and the other half into another.
		// The validationmodifier must use the list that starts on the second to last digit. Hence, an offset. 
		int offset = 0;
		if (IsEven(parsedCardNumber.Length - 1))
			offset = 1;

		int validationModifier = 0;
		int validationSum = 0;

		for (int i = parsedCardNumber.Length - 1; i >= 0; i--)
		{

			if (IsEven(i + offset))
			{
				// If this is the list starting on the 2nd to last digit:
				// Multiply the value by 2 make it a string.
				// This is because if we get a double digit number, say 16
				// We want to read it as 1, 6; not 16.
				string s = (parsedCardNumber[i] * 2).ToString();

				for (int j = 0; j < s.Length; j++)
				{
					// Now we have the value as a string (say 16)
					// Go through all the numbers in that string (1, 6) and add them to the validation modifier.
					// In the example: validationModifier += 1; /Next loop/ validationModifier += 6;
					validationModifier += int.Parse(s[j].ToString(), NumberStyles.Integer);
				}

			}
			else
			{
				// If this is the list starting on the last digit, just add it.
				validationSum += parsedCardNumber[i];
			}
		}

		// Add the sum of the 2 lists after modification together and make it a string.
		// This is because we ONLY want to see if the last digit of whatever number we get is 0. 
		string validationNumber = (validationSum + validationModifier).ToString();

		// If the last number is equal to 0 we have a valid card!
		if (validationNumber[validationNumber.Length - 1] == '0')
			return true;

		// Since we didn't return the card can't end in 0 and is not valid.
		return false;
	}


	// We use a custom bitwise AND operator to check if a number is odd or even.
	// This is faster (and cooler) than modulus.
	static bool IsEven(int n)
	{
		return ((n & 1) != 1);
	}
}