namespace Embers
{
    public static class InGameChatNoticeHandler
    {
        private static ChatUIController chatUIController;

        public static void Register(ChatUIController controller)
        {
            chatUIController = controller;
        }

        public static void Unregister()
        {
            chatUIController = null;
        }

        public static void Notice(string header, string message)
        {
            chatUIController.AddChatMessageHandle($"[{header}]", message);
        }
    }
}
