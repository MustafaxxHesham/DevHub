using DevHub.Domain.Enums;
using DevHub.Domain.Models;
using Newtonsoft.Json;

namespace DevHub.Utilities;
public static class IdempotencyStateHandler
{
    extension(string str)
    {
        public string CompleteIdempontentString()
        {
            var idempotencyObject = JsonConvert.DeserializeObject<Idempotency>(str);
            idempotencyObject!.State = IdempotencyState.Completed;
            return JsonConvert.SerializeObject(idempotencyObject);
        }
    }
}