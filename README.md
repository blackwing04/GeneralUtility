# 通用工具規劃與使用說明

## 專案簡介
本專案旨在創建通用的底層資料庫工具，方便開發人員快速實現資料庫 DAO 功能。專案包含多種工具，支援多個框架和版本（目前適配 .NET 6），並具備高度可擴展性，適用於不同需求。

---

## 功能
- **靜態工具（StaticUtil）：** 提供通用方法、模型和列舉，無需實例化即可直接使用。
- **自定義例外（CustomizeException）：** 提供自定義例外模組，可方便地建立和擴展例外處理邏輯。
- **通用資料庫 DAO（DAO）：** 支援 SQLite 和 MSSQL，未來可擴充其他資料庫引擎。
- **Excel 工具（ExcelTool）：** 提供 DataTable 與 Excel 間的互轉功能。
- **單元測試：** 提供 DAO 和 Excel 工具的單元測試模組。

---

## 安裝與使用
### **環境需求**
- 開發環境：.NET 6
- 必要參考：
  - `Generic.StaticUtil`
  - `CustomizeException`

### **安裝步驟**
1. 複製專案：
   - 複製專案資料夾至主專案目錄中。
2. 新增至解決方案：
   - 在主專案中將複製的專案新增至解決方案。
3. 加入專案參考：
   - 參考所需的模組（如 `DAO`、`Generic.StaticUtil`）。

---

## 專案結構
- **StaticUtil：** 提供靜態方法與工具。
- **CustomizeException：** 自定義例外處理模組。
- **DAO：** 資料庫訪問層（支援 SQLite 和 MSSQL）。
- **DAO.Test：** 資料庫 DAO 的單元測試。
- **ExcelTool：** 提供 Excel 與 DataTable 的互轉功能。
- **ExcelTool.Test：** Excel 工具的單元測試。

---
