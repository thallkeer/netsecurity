namespace ClientForLab
{
    public static class Commands
    {
        public const string InitTransmition   = "Собираюсь отправить файл";
        public const string SendEncFile = "Отправляю зашифрованный файл";
        public const string SendKey = "Отправляю ключ";
        public const string SendHash = "Отправляю хеш";
        public const string HashRequest = "Запрос хеша";
        public const string SendVector = "Отправляю вектор";
        public const string KeyRequest = "Запрос ключа";
        public const string VectorRequest = "Запрос вектора";
        public const string FileRequest = "Запрос файла";
        public const string Handshake = "Подтверждение приема";
        public const string EndTransmition = "Конец передачи";
        public const string PublicKeyRequest = "Запрос ключа rsa";
        public const string SendPublicKey = "Отправляю ключ rsa";
    }
}
