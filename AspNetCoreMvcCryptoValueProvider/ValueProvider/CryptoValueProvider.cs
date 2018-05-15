using AspNetCoreMvcCryptoValueProvider.ValueProvider;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

namespace AspNetCoreMvcCryptoValueProvider
{
    public class CryptoValueProvider : IValueProvider
    {
        string _encryptedParameters;
        //CryptoParamsProtector _protector;
        Dictionary<string, string> _values;

        public CryptoValueProvider(string encryptedParameters)

        {
            _encryptedParameters = encryptedParameters;
            //_protector = protector;
        }

        public bool ContainsPrefix(string prefix)
        {
            if (_values == null)
            {
                if (string.IsNullOrEmpty(_encryptedParameters))
                {
                    _values = new Dictionary<string, string>();
                }
                else
                {
                    _values = CryptoNew.DecryptInKeyValue(_encryptedParameters);
                }
            }

            return _values.ContainsKey(prefix.ToUpper());
        }

        public ValueProviderResult GetValue(string key)
        {
            if (_values.ContainsKey(key.ToUpper()))
            {
                return new ValueProviderResult(new StringValues(_values[key.ToUpper()]));
            }
            else
            {
                return ValueProviderResult.None;
            }
        }
    }
}
