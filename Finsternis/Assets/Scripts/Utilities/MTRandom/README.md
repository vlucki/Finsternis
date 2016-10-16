#MTRANDOM

###Random Numbers for Unity3d

In Unity3d there is already a Random number generator based on the platform-specific random generator. 
Here we present an alternative Random library for Unity3d designed to generate uniform Pseudo-Random deviates. The library use a fast PRNG (Mersenne-Twister) to generate: Floating Number in range [0-1] and in range [n-m], Vector2 and Vector3 and Color data types.The uniform deviates can be transformed with the distributions: Standard Normal Distribution and Power-Law. 
In addition is possible to generate floating random deviates coming from other distributions: Poisson, Exponential and Gamma.


```
using UnityEngine;
using UMT;

public class MTRandomExample : MonoBehaviour 
{
	private MTRandom mrand;
	
	// seed can be also an int
	public string seed = "Test Seed"

	void Start () 
	{
		mrand = new MTRandom(seed);
	}

	void Update()
	{
	    float x = mrand.value()
	    float y = mrand.valueNorm(1.0f);
	    float z = mrand.valuePower(1.0f);
	}
}
```

See pdf documentation for more info on main class **MTRandom**



