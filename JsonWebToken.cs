using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;

namespace SPOAuthFiddlerExt {
  public class JsonWebToken {
    public static string[] Decode(string token) {

      var parts = token.Split('.');
      string[] ret = new string[2];

      var header = parts[0];
      var payload = parts[1];

      var headerJson = Encoding.UTF8.GetString(Base64UrlDecode(header));
      ret[0] = headerJson;

      string payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(payload));
      ret[1] = payloadJson;

      return ret;
    }

    public static IReadOnlyDictionary<string, object> Deserialize(string token) {
      JavaScriptSerializer ser = new JavaScriptSerializer();
      var dict = ser.Deserialize<dynamic>(token);
      return dict;
    }


    private static byte[] Base64UrlDecode(string value) {
      string convert = value;

      switch (convert.Length % 4) {
        case 0:
          break;
        case 2:
          convert += "==";
          break;
        case 3:
          convert += "=";
          break;
        default:
          throw new System.Exception("oops");
      }
      var ret = Convert.FromBase64String(convert);
      return ret;
    }

  }

}


