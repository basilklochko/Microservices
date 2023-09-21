using Common.Implementation;
using Common.Interface;
using HotelService.Handler;
using Kafka.BackgroundService;
using Kafka.Implementation;
using Kafka.Interface;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddSingleton<ITopic, Topic>();
builder.Services.AddSingleton<IHandler, HotelHandler>(); 
builder.Services.AddSingleton<IPublisher, Publisher>();
builder.Services.AddSingleton<ISubscriber, Subscriber>();
builder.Services.AddHostedService<SubscribeService>(x => new SubscribeService(x.GetService<ISubscriber>(),
    x.GetService<IConfiguration>().GetSection("Kafka").GetSection("Topic").Value,
    x.GetService<IConfiguration>().GetSection("Kafka").GetSection("CancelTopic").Value,
    x.GetService<IConfiguration>().GetSection("Kafka").GetSection("ConfirmTopic").Value,
    x.GetService<IConfiguration>().GetSection("Kafka").GetSection("FailTopic").Value,
    x.GetService<IHandler>()));



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(policy =>
{
    policy.AddPolicy("CorsPolicy", opt => opt
        .WithOrigins("*")
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors("CorsPolicy");

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<HotelHub>("/hotel");
});

await app.Services.GetRequiredService<ITopic>().Create(new string[]
{
    app.Services.GetService<IConfiguration>().GetSection("Kafka").GetSection("Topic").Value,
    app.Services.GetService<IConfiguration>().GetSection("Kafka").GetSection("CancelTopic").Value,
    app.Services.GetService<IConfiguration>().GetSection("Kafka").GetSection("ConfirmTopic").Value,
    app.Services.GetService<IConfiguration>().GetSection("Kafka").GetSection("FailTopic").Value
});

await app.RunAsync();

