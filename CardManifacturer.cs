public class CardManifacturer
{
	// I used public readonly variables instead of properties because I was lazy - sorry.
	public readonly string name = "Manifacturer name";
	public readonly int numbersToCheck = 2;
	public readonly string[] definitions = null;
	
	public CardManifacturer(string name, int numbersToCheck, string[] definitions)
	{
		this.name = name;
		this.numbersToCheck = numbersToCheck;
		this.definitions = definitions;
	}
}