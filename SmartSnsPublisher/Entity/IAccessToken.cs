using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartSnsPublisher.Entity
{
    public interface IAccessToken
    {
        string UserId { get; set; }
        string AccessToken { get; set; }
        int Expire { get; set; }
        string Error { get; set; }
    }
}
