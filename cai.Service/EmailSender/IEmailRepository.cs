using System.Threading.Tasks;
using System.IO;

namespace cai.Service.EmailSender
{
    public interface IEmailRepository
    {
        Task SendB2bStock(FileStream file);
    }
}
