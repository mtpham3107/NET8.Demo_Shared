//using DotNetCore.CAP;
//using Microsoft.AspNetCore.Mvc;
//using NET8.Demo.RabbitMQ.Etos;
//using static NET8.Demo.RabbitMQ.RabbitMQTopic;

//namespace NET8.Demo.GlobalAdmin.Controllers.Api;

//[Route("api/notifications")]
//public class NotificationController : ApiControllerBase
//{
//    private readonly ICapPublisher _capPublisher;

//    public NotificationController(ICapPublisher capPublisher)
//    {
//        _capPublisher = capPublisher;
//    }

//    [HttpPost("send")]
//    public async Task<IActionResult> SendNotification([FromBody] string message)
//    {
//        var eto = new NotificationSendEto()
//        {
//            Recipient = UserId,
//            Title = "Test",
//            Message = message,
//            IsRead = false,
//            CreatedBy = UserId,
//        };

//        await _capPublisher.PublishAsync(NOTIFICATION_SEND, eto);
//        return Ok();
//    }
//}
