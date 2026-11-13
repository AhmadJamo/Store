# المهمة التالية الوحيدة / Next Task

## MOD-030Z — تنفيذ أول دفعة من عشرة Controllers

> الحالة: APPROVED FOR EXECUTION

نفّذ تقاعد المجموعة المدققة في `MOD-030Y` كدفعة واحدة فقط:

1. SupplierPayments
2. CustomerPayments
3. PurchaseReplenishment
4. ProductAttributes
5. ProductStocks
6. SalesDeliveries
7. ProductTemplates
8. Replenishment
9. Suppliers
10. InventoryScanning

احذف Controllers وRazor التابعة بعد نقل GET/HEAD الدقيقة إلى retired catalog. يجب أن تعيد MVC POSTs القديمة 405،
وأن تبقى APIs وAngular وApplication services والصلاحيات وantiforgery وقواعد المحاسبة والمخزون كما هي. نفّذ Release
والفحوص الكاملة والحدود الحية والتوثيق ثم كومت واحد للمجموعة كلها. لا تدخل أي Controller حادي عشر.
