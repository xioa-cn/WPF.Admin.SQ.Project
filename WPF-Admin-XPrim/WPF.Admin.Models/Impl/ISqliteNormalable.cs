using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF.Admin.Models.Impl
{
    public interface ISqliteNormalable
    {
        public Task DbFileExistOrCreate();
    }
}
