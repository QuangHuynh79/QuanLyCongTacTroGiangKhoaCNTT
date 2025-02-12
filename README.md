# 📚 Teaching Assistant Management System VLU  

A system for managing teaching assistants at the Faculty of Information Technology, Van Lang University (VLU).  

## 🚀 Introduction  

The **Teaching Assistant Management System VLU** is designed to help track, assign, and manage teaching assistants for courses in the Faculty of Information Technology. The system provides features such as:  

- Managing teaching assistant information.  
- Registering assistants for courses.  
- Assigning assistants based on schedules.  
- Monitoring and evaluating assistant performance.  
- Supporting image uploads and file exports.  

## 📂 Project Structure  

```
📦 Teaching Assistant Management System VLU  
├── 📂 Config                 # System configuration  
├── 📂 Controllers            # Controllers for handling logic  
├── 📂 Models                 # Data models  
├── 📂 Views                  # User interface views  
├── 📂 Scripts                # Supporting scripts  
├── 📂 Validation             # Data validation handling  
├── 📂 Middleware             # Role-based access control  
├── 📂 Content                # Handles image uploads and file exports  
├── 📂 QuanLyCongTacTroGiangKhoaCNTTTest  # Unit test for core functionalities  
│   ├── 📜 TestClass.cs       # Unit tests for class logic  
│   ├── 📜 TestManagement.cs  # Unit tests for assistant management  
│   ├── 📜 TestAPI.cs         # API endpoint testing  
│   ├── 📜 TestDatabase.cs    # Database connection testing  
│   ├── 🌐 test-config.json   # Test configuration settings  
├── 📝 README.md              # Documentation  
├── 🌍 Global.asax.cs         # ASP.NET application configuration  
└── 🌐 Web.config             # Web configuration  
```  

## 🛠️ Technologies Used  

- **ASP.NET MVC** - Primary framework  
- **C#** - Backend programming language  
- **HTML, CSS, JavaScript** - Frontend development  
- **SQL Server** - Database management  
- **xUnit / NUnit** - Unit testing framework  

## 🧪 Unit Testing  

The `QuanLyCongTacTroGiangKhoaCNTTTest` folder contains unit tests for key system functionalities. The tests focus on:  

✅ **Class Logic Tests**: Ensuring that business logic behaves as expected.  
✅ **Management Functionality Tests**: Verifying the registration and assignment of assistants.  
✅ **API Endpoint Tests**: Checking the correctness of API responses.  
✅ **Database Connection Tests**: Ensuring the database integration works properly.  

### 📌 Running Unit Tests  

To execute the tests, follow these steps:  

1. **Open the project in Visual Studio**.  
2. **Ensure NUnit/xUnit is installed** (via NuGet).  
3. **Run tests using Test Explorer**:  
   - Go to `Test > Test Explorer`.  
   - Click **Run All** to execute all tests.  
   - Verify results in the **Test Explorer Panel**.  

Alternatively, use the command line:  

```sh  
dotnet test QuanLyCongTacTroGiangKhoaCNTTTest  
```
## 🖥️ Main Interface  

![Main Website UI](https://github.com/user-attachments/assets/a1ff9297-c070-4fab-89f4-75b6bf03ba4e)  

The homepage features a **login system using VLU Email**, ensuring security and authentication for students and faculty members.  
## 🌐 Live Website  

🔗 **Access the system here:** [Teaching Assistant Management System VLU](https://cntttest.vanlanguni.edu.vn:18081/CAP24Team02)  

## 🔑 Test Accounts  

| Username                   | Password      | Role       |  
|----------------------------|---------------|------------|  
| k.cntt-test1@vanlanguni.vn | cntt@Test1    | Admin      |  
| k.cntt-test2@vanlanguni.vn | cntt@Test2    | Assistant  |  

## 📌 Installation Guide  

1. **Clone the repository**:  
   ```sh  
   git clone https://github.com/QuangHuynh79/QuanLyCongTacTroGiangKhoaCNTT.git  
   ```  
2. **Open the project in Visual Studio**.  
3. **Configure the database** in `Web.config`.  
4. **Run the project** by pressing `F5`.  

## 🤝 Contribution  

If you would like to contribute, please fork this repository and create a pull request!  

## 📜 License  

This project is licensed under the **`Document Promulgating Regulations on organization and management of Teaching Assistant activities at Van Lang University, Decision No. 1352/QD-DHVL-DT, issued on August 27, 2024` License**.  

## 👨‍💻 Development Team  

| Member       | GitHub Username      | Role                 |  
|--------------|----------------------|----------------------|  
| Huỳnh Quang  | [QuangHuynh79](https://github.com/QuangHuynh79) | Developer |  
| Minh Quang   | [Quang-Duong](https://github.com/Quang-Duong) | Scrum Master |  
| Mạnh Tài     | [ManhTai1001](https://github.com/ManhTai1001) | Business Analyst (BA) |  
| Duy Toàn     | [duytoan2205](https://github.com/duytoan2205) | Developer & Tester |  
| Thế Tài      | [diepthetai27](https://github.com/diepthetai27) | Developer |  


📧 Contact: `k.cntt@vlu.edu.vn`  
📞 Hotline: `028.71099240`  


















