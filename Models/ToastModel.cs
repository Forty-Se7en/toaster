namespace Notification.Models
{
    public class ToastModel
    {
        public string Source { get; set; } = null;

        public string Header { get; set; } = null;
        public string Title { get; set; } = null;

        public string Message { get; set; } = null;
        public string Text { get; set; } = null;
        public string Label { get; set; } = null;

        public string Logo { get; set; } = null;
        public string Image { get; set; } = null;

        public string HeroImage { get; set; } = null;

        public bool Sound { get; set; }

        public string Xml { get; set; } = null;

        public CommandModel Command { get; set; }

        public CommandModel OnClick { get; set; }

        public ToastModel Clone()
        {
            return new ToastModel
            {
                Source = Source,
                Header = Header,
                Title = Title,
                Message = Message,
                Text = Text,
                Label = Label,
                Logo = Logo,
                Image = Image,
                HeroImage = HeroImage,
                Sound = Sound,
                Xml = Xml,
                Command = Command?.Clone(),
                OnClick = OnClick?.Clone()
            };
        }
    }
}
