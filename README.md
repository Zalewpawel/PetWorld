# 🐾 PetWorld

AI-powered pet store chat application built with **Blazor** and **GPT-4o**. Intelligent chatbot with product recommendations using Writer-Critic agent pattern.

## ✨ Features

- 💬 AI-powered chat with GPT-4o (Writer-Critic pattern)
- 📊 Chat history with full message details
- 🛍️ Product catalog with detail views
- 🔗 Auto-linked products in AI responses
- 📱 Responsive UI with animations
- 🔐 Secure credential management

## 🛠️ Tech Stack

- **.NET 9.0** - Framework
- **Blazor Interactive Server** - UI
- **Entity Framework Core** - ORM
- **MySQL 8.0** - Database
- **GPT-4o API** - AI engine

## 🚀 Quick Start

### Prerequisites
- .NET 9.0 SDK
- MySQL 8.0 running locally
- OpenAI API Key

### Setup
```bash
git clone https://github.com/Zalewpawel/PetWorld.git
cd PetWorld

# Set OpenAI API key
cd PetWorld.Web
dotnet user-secrets init
dotnet user-secrets set "OpenAI:ApiKey" "your-api-key-here"
cd ..

# Build and run
dotnet build
dotnet run --project PetWorld.Web
```

Access at **http://localhost:5000**

## 📖 Usage

1. **Chat** - Ask questions about pet products, AI will recommend relevant items with product links
2. **History** - View past conversations, click any message for full details
3. **Products** - Browse catalog, click products to see full information

## 🏗️ Architecture

**Clean Architecture** with 4 layers:
- **Domain** - Entities & interfaces (ChatMessage, Product)
- **Application** - Services (ChatService, ProductService)
- **Infrastructure** - Repositories & AI (AppDbContext, AgentService)
- **Web** - Blazor UI components

## ⚙️ Configuration

- **appsettings.json** - Connection strings, logging
- **User Secrets** - OpenAI API key (local development only)

## 🤖 AI Agent Pattern

**Writer-Critic Loop** (up to 3 iterations):
1. **Writer** - Generates answer with product recommendations
2. **Critic** - Validates accuracy and product references
3. If approved → Return answer | Otherwise → Loop with feedback

## 📁 Project Structure

```
PetWorld/
├── PetWorld.sln
├── PetWorld.Domain/              # Core entities & interfaces
├── PetWorld.Application/         # Business services
├── PetWorld.Infrastructure/      # Repositories, DB, AI
└── PetWorld.Web/                 # Blazor UI
    ├── Components/Pages/
    │   ├── Chat.razor
    │   ├── History.razor
    │   ├── Products.razor
    │   ├── MessageDetails.razor
    │   └── ProductDetails.razor
    ├── Program.cs
    └── appsettings.json
```

## 🔧 Development

```bash
# Build
dotnet build

# Run
dotnet run --project PetWorld.Web

# Create migration
cd PetWorld.Infrastructure
dotnet ef migrations add MigrationName --startup-project ../PetWorld.Web
```

## 🔐 Security

- API keys stored in User Secrets (not in source control)
- Clean .gitignore excluding build artifacts
- Secure configuration with environment variables

## 📚 Services

### ChatService
- `SendMessageAsync(question)` - Process question, return AI response
- `GetAllMessagesAsync()` - Retrieve chat history
- `GetMessageByIdAsync(id)` - Get message details

### ProductService
- `GetAllProductsAsync()` - List all products
- `GetProductByIdAsync(id)` - Get product details

## 🤝 Contributing

1. Fork repository
2. Create feature branch (`git checkout -b feature/name`)
3. Commit changes (`git commit -m 'Add feature'`)
4. Push (`git push origin feature/name`)
5. Open Pull Request

## 📝 License

Educational & commercial use allowed.

## 🎯 Roadmap

- [ ] GitHub Actions CI/CD
- [ ] Unit tests
- [ ] Swagger API docs
- [ ] Performance optimizations

## 📞 Contact

**Developer**: Paweł Zalewski  
**Repository**: https://github.com/Zalewpawel/PetWorld

---

Built with ❤️ using .NET 9.0 and Blazor
