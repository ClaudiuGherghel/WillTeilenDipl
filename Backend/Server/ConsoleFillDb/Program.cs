using Core.Contracts;
using Core.Helper;
using Persistence;
using System.Threading.Tasks;


namespace ConsoleFillDb
{
    internal class Program
    {
        static async Task Main()
        {
            //Generate Secret Key for apsettings.json
            SecretKeyHelper.GenerateSecretKeyGenerator();

            //Fill Db with Mock data
            using UnitOfWork uow = new();
            await uow.FillDbAsync();
            Console.WriteLine("Anzahl Kategorien: " + await uow.CategoryRepository.CountAsync());
            Console.WriteLine("Anzahl Subkategorien: " + await uow.SubCategoryRepository.CountAsync());
            Console.WriteLine("Anzahl User: " + await uow.UserRepository.CountAsync());
        }




    }
}
