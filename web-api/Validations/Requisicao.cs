namespace web_api.Validations
{
    public class Requisicao
    {
        public static bool IdRequisicaoIgualAoIdCorpoRequisicao(int idRequisicao, int idCorpoRequisicao)
        {
            return idRequisicao == idCorpoRequisicao;
        }
    }
}