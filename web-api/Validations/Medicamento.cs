using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ModelBinding;

namespace web_api.Validations
{
    public class Medicamento
    {
        public static bool DataVencimentoMaiorQueDataFabricacao(Models.Medicamento medicamento)
        {
            return medicamento.DataVencimento > medicamento.DataFabricacao;            
        }
    }
}