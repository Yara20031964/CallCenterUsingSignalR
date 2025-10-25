using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace CallCenterUsingSignalR_2B.Controllers
{
    public class CallController : Controller
    {
        private readonly IHubContext<CallHub> _hubContext;

        public CallController(IHubContext<CallHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SimulateCall(string agentId = "Agent1")
        {
            var status = CallHub.GetAgentStatus(agentId);

            // نجيب ConnectionId للـ Agent المحدد
            var connectionId = CallHub.GetConnectionId(agentId);

            if (status != "avaliable" && connectionId != null)
                return Ok($"Agent {agentId} is busy. Cannot simulate call.");
            // dah 3ahan 23aml simulate ani fi rakm gdid bytasl
            var random = new Random();
            string phoneNumber = "01";
            for (int i = 0; i < 9; i++)
            {
                phoneNumber += random.Next(0, 10).ToString();
            }


            if (connectionId != null)
            {
                await _hubContext.Clients.Client(connectionId)
                    .SendAsync("ReceiveCall", phoneNumber);
                return Ok($"Call sent to {agentId} with number {phoneNumber}");
            }
            else
            {
                return NotFound($"Agent {agentId} not connected");
            }
        }
    }
}
