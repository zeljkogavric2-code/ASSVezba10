
using System.Windows;
using OrderSystem.Orders;
using OrderSystem.Persistence.Infrastructure;
using OrderSystem.Notifications.Infrastructure;
using OrderSystem.Shared.Infrastructure;
using OrderSystem.Pricing;
using OrderSystem.SharedKernel;
using System.Collections.Generic;
using System.Linq;
using OrderSystem.Notifications.Application;

namespace OrderSystem.UI
{
    public partial class MainWindow : Window
    {
        private readonly OrderQueryService _queryService;
        private readonly OrderCommandService _commandService;
        private List<OrderItem> items = new List<OrderItem>();

        public MainWindow()
        {
            InitializeComponent();

            var repo = new JsonOrderRepository();
            var email = new EmailService();
            var time = new SystemDateTimeProvider();
            var id = new GuidGenerator();
            var pricing = new PricingService();
            var eventBus = new InMemoryEventBus();
            _queryService = new OrderQueryService(repo);
            _commandService = new OrderCommandService(repo, time, id, pricing,eventBus);

            var handler = new NotificationHandler(eventBus, email);

            lstOrders.ItemsSource = _queryService.GetAllOrders();
        }

        private void CreateOrder_Click(object sender, RoutedEventArgs e)
        {
            _commandService.CreateOrder(txtCustomer.Text, items);            

            items = new List<OrderItem>();
            lstOrders.ItemsSource = _queryService.GetAllOrders();
        }

        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            var item = new OrderItem
            {
                Product = txtProduct.Text,
                Price = decimal.Parse(txtPrice.Text),
                Quantity = int.Parse(txtQty.Text)
            };

            items.Add(item);
        }
    }
}
