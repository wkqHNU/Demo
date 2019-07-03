using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallDemo.Infrastructure.Template
{
    public class GlobalParameter
    {
        private string pathlossFilePath;
        private string thetaModel;
        private string phiModel;

        public string PathlossFilePath { get => pathlossFilePath; set => pathlossFilePath = value; }
        public string ThetaModel { get => thetaModel; set => thetaModel = value; }
        public string PhiModel { get => phiModel; set => phiModel = value; }
    }
}
