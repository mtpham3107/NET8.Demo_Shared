//using AutoMapper;
//using DotNetCore.CAP;
//using Microsoft.AspNetCore.SignalR;
//using Microsoft.Extensions.Logging;
//using NET8.Demo.GlobalAdmin.Application.Contracts.IServices;
//using NET8.Demo.GlobalAdmin.Application.Contracts.Responses;
//using NET8.Demo.GlobalAdmin.Application.SignalRHubs;
//using NET8.Demo.GlobalAdmin.Domain.Entities;
//using NET8.Demo.GlobalAdmin.Domain.IUnitOfWorks;
//using NET8.Demo.RabbitMQ.Etos;
//using static NET8.Demo.RabbitMQ.RabbitMQTopic;
//using static Newtonsoft.Json.JsonConvert;

//namespace NET8.Demo.GlobalAdmin.Application.Handler;

//public class NotificationHandler : ICapSubscribe
//{
//    private readonly ILogger<NotificationHandler> _logger;
//    private readonly IUnitOfWork _unitOfWork;
//    private readonly IHubContext<NotificationHub> _hubContext;
//    private readonly IUserService _userService;
//    private readonly IMapper _mapper;

//    public NotificationHandler(
//        ILogger<NotificationHandler> logger,
//        IUnitOfWork unitOfWork,
//        IHubContext<NotificationHub> hubContext,
//        IUserService userService,
//        IMapper mapper)
//    {
//        _logger = logger;
//        _unitOfWork = unitOfWork;
//        _hubContext = hubContext;
//        _userService = userService;
//        _mapper = mapper;
//    }

//    [CapSubscribe(NOTIFICATION_SEND)]
//    public async Task Subscibe(NotificationSendEto eventData)
//    {
//        _logger.LogInformation("NotificationHandler-Subscibe: {eventData}", SerializeObject(eventData));
//        await _unitOfWork.Repository<Notification>().InsertAsync(eventData.CreatedBy, _mapper.Map<Notification>(eventData));
//        await _unitOfWork.SaveChangesAsync();

//        var notifications = await _unitOfWork.Repository<Notification>().GetListAsync(x => !x.IsRead && x.Recipient == eventData.Recipient);

//        var senderIds = notifications.Select(x => x.CreatedBy.GetValueOrDefault()).ToHashSet();

//        var senders = await _userService.GetByIdsAsync(senderIds);

//        var result = notifications
//            .Join(senders,
//                notification => notification.CreatedBy,
//                sender => sender.Id,
//                (notification, sender) => new NotificationResponse()
//                {
//                    Title = notification.Title,
//                    Message = notification.Message,
//                    Link = notification.Link,
//                    IsRead = notification.IsRead,
//                    Sender = sender,
//                    CreatedDate = notification.CreatedDate,
//                });

//        await _hubContext.Clients.User(eventData.Recipient.ToString()).SendAsync("ReceiveNotification", result);
//    }
//}
