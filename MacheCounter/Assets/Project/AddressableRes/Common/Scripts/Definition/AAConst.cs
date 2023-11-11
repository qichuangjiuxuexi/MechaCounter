using System.Collections.Generic;
public class AAConst
{
	public static string FloatValueConfig = "Data.FloatValueConfig.asset";
	public static string IntListValueConfig = "Data.IntListValueConfig.asset";
	public static string IntValueConfig = "Data.IntValueConfig.asset";

	public static Dictionary<string,string> keyAddressDict = new Dictionary<string,string>()
	{
		{"FloatValueConfig" , FloatValueConfig},
		{"IntListValueConfig" , IntListValueConfig},
		{"IntValueConfig" , IntValueConfig},
	};

	public static string GetAddress(string key)
	{
		if(keyAddressDict.TryGetValue(key, out var address))
		{
			return address;
		}
		return "";
	}

}
