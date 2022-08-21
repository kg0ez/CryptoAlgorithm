using Algorithm;

var lastInstrument = "USDT";
int count = 1;

CryptoAlgorithm crypto = new CryptoAlgorithm();

List<string> chains = await crypto.GetChains(lastInstrument, count);

Console.ReadLine();