namespace Algorithm
{
	public class CryptoAlgorithm
	{
        public async Task<List<string>> GetChains(string lastInstrument, int count)
        {
            count -= 2;
            var api = new CCXT.NET.Poloniex.Public.PublicApi();

            var markets = await api.FetchMarketsAsync();
            var tickers = markets.result;

            Dictionary<string, string> firstPairs = GetFirstPairs(lastInstrument, tickers);
            Dictionary<string, string> pairsWithoutLastInstryments = new Dictionary<string, string>();
            Dictionary<string, string> chainsReadyInstryment = new Dictionary<string, string>();
            Dictionary<string, string> chainsWithoutLastPair = new Dictionary<string, string>();

            if (count == 1)
            {
                var chains = GetShortChains(tickers, lastInstrument, firstPairs);
                return chains;
            }
            else if( count < 1)
            {
                var pairs = new List<string>();

                foreach (var pair in firstPairs)
                    pairs.Add(pair.Key);

                return pairs;
            }

            foreach (var firstPair in firstPairs)
            {
                string instrument = firstPair.Value;
                for (int i = 0; i < count; i++)
                {
                    if (i == 0)
                    {
                        pairsWithoutLastInstryments = GetPairs(instrument, lastInstrument, tickers);
                    }
                    else
                    {
                        if (i > 1)
                        {
                            pairsWithoutLastInstryments = chainsReadyInstryment;
                            chainsReadyInstryment = new Dictionary<string, string>();
                        }
                        chainsReadyInstryment = GetAlmostReadyChains(instrument, lastInstrument, tickers, pairsWithoutLastInstryments);
                    }
                    if (i >= count - 1)
                    {
                        foreach (var chain in chainsReadyInstryment)
                        {
                            chainsWithoutLastPair.Add(firstPair.Key + " " + chain.Key, chain.Value);
                        }

                        chainsReadyInstryment = new Dictionary<string, string>();
                        pairsWithoutLastInstryments = new Dictionary<string, string>();
                    }
                }
            }

            var readyChains = GetChains(tickers, lastInstrument, chainsWithoutLastPair);

            return readyChains;
        }

        private Dictionary<string, string> GetFirstPairs(string lastInstrument, Dictionary<string, CCXT.NET.Shared.Coin.Public.IMarketItem> tickers)
        {
            Dictionary<string, string> firstPairs = new Dictionary<string, string>();
            foreach (var ticker in tickers)
            {
                if (ticker.Value.quoteId == lastInstrument)
                {
                    firstPairs.Add(ticker.Key, ticker.Value.baseId);
                }
            }
            return firstPairs;
        }

        private Dictionary<string,string> GetPairs(string instrument,string lastInstrument,
            Dictionary<string, CCXT.NET.Shared.Coin.Public.IMarketItem> tickers)
        {
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            foreach (var ticker in tickers)
            {
                if (ticker.Value.baseId == instrument && ticker.Value.quoteId != lastInstrument)
                {
                    pairs.Add(ticker.Key, ticker.Value.quoteId);
                }
            }
            return pairs;
        }

        private Dictionary<string,string> GetAlmostReadyChains(string instrument, string lastInstrument,
            Dictionary<string, CCXT.NET.Shared.Coin.Public.IMarketItem> tickers, Dictionary<string,string> pairsWithoutLastInstryments)
        {
            Dictionary<string, string> chains = new Dictionary<string, string>();

            foreach (var pair in pairsWithoutLastInstryments)
            {
                instrument = pair.Value;
                foreach (var ticker in tickers)
                {
                    if (ticker.Value.baseId == instrument && ticker.Value.quoteId != lastInstrument)
                    {
                        chains.Add(pair.Key + " " + ticker.Key, ticker.Value.quoteId);
                    }
                }
            }
            return chains;
        }

        private List<string> GetChains(Dictionary<string, CCXT.NET.Shared.Coin.Public.IMarketItem> tickers,
            string lastInstrument, Dictionary<string,string> chains)
        {
            var readyChains = new List<string>();

            foreach (var item in chains)
            {
                foreach (var ticker in tickers)
                {
                    if (ticker.Key == $"{item.Value}/{lastInstrument}")
                    {
                        readyChains.Add(item.Key + $" {item.Value}/{lastInstrument}");
                        break;
                    }
                }
            }
            return readyChains;
        }

        private List<string> GetShortChains(Dictionary<string, CCXT.NET.Shared.Coin.Public.IMarketItem> tickers,
            string lastInstrument, Dictionary<string, string> firstPairs)
        {
            List<string> chains = new List<string>();
            foreach (var firstPair in firstPairs)
            {
                Dictionary<string, string> secondTickers = new Dictionary<string, string>();
                foreach (var ticker in tickers)
                {
                    if (ticker.Value.baseId == firstPair.Value && ticker.Value.quoteId != lastInstrument)
                    {
                        foreach (var item in tickers)
                        {
                            if (item.Key == $"{ticker.Value.quoteId}/{lastInstrument}")
                            {
                                chains.Add(firstPair.Key + " " + ticker.Key + " " + item.Key);
                            }
                        }
                    }
                }
            }
            return chains;
        }
	}
}

