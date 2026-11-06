import { inject, Injectable } from '@angular/core';
import { LocalizationService, MessageKey } from '../../core/localization/localization.service';

const navigationMessages = {
  'ar-JO': {
    manualJournals: 'القيود اليدوية',
    financialReports: 'التقارير المالية',
    agingReports: 'أعمار الذمم والمصالحة',
    purchasingCenter: 'مركز المشتريات', purchasingCenterDescription: 'مساحة موحدة للتخطيط والتوريد والاستلام والذمم الدائنة وعمليات الموردين.', salesCenter: 'مركز المبيعات', salesCenterDescription: 'مساحة موحدة للتسعير والطلبات والتسليم والتحصيل والمرتجعات والبيع الفوري.', accountingCenter: 'مركز المحاسبة', accountingCenterDescription: 'وصول منظم إلى شجرة الحسابات وإعدادات المحاسبة والفترات والعملات وربط الفروع.', noModuleDestinations: 'لا توجد وجهات متاحة', noModuleDestinationsDescription: 'لا تملك صلاحية لفتح أي مساحة عمل في هذه الوحدة.', openWorkspace: 'فتح مساحة العمل',
    planningAndSourcing: 'التخطيط والتوريد', receivingAndFulfillment: 'الاستلام والتنفيذ', payablesAndControl: 'الذمم الدائنة والرقابة', supplierOperations: 'عمليات الموردين', commercialPreparation: 'التجهيز التجاري', salesFulfillment: 'تنفيذ المبيعات', receivablesAndCollection: 'الذمم المدينة والتحصيل', returnsAndCredits: 'المرتجعات والإشعارات الدائنة', immediateSales: 'البيع الفوري', accountingSetup: 'الإعداد المحاسبي',
    purchaseApprovalRules: 'قواعد اعتماد الشراء', purchaseSourcingWorkspace: 'التوريد وطلبات العروض', purchaseOrders: 'أوامر الشراء', purchaseAgreements: 'اتفاقيات الشراء', purchaseTemplates: 'قوالب الشراء', purchaseReplenishment: 'تجديد مخزون الشراء', goodsReceipts: 'إيصالات البضاعة', serviceAcceptances: 'استلام الخدمات', purchaseReturns: 'مرتجعات الاستلام', landedCosts: 'التكاليف الإضافية', vendorBills: 'فواتير الموردين', supplierCreditNotes: 'إشعارات المورد الدائنة', supplierPayments: 'دفعات الموردين', purchaseReversals: 'عكس مستندات الشراء', purchaseAccountingReconciliation: 'مطابقة محاسبة الشراء', supplierTerms: 'شروط الموردين', supplierTermExceptions: 'استثناءات شروط الموردين', supplierCommunications: 'مراسلات الموردين', supplierIntegrations: 'تكاملات الموردين', purchaseAnalytics: 'تحليلات الشراء', legacyPurchases: 'المشتريات المباشرة', legacyPurchaseReturns: 'مرتجعات الشراء المباشر',
    accountingReview: 'المراجعة المحاسبية', journalEntries: 'دليل القيود اليومية', salesPricing: 'تسعير المبيعات', salesQuotationTemplates: 'قوالب عروض الأسعار', salesQuotations: 'عروض الأسعار', salesApprovalRules: 'قواعد اعتماد المبيعات', salesOrders: 'أوامر البيع', salesDeliveries: 'تسليمات المبيعات', customerInvoices: 'فواتير العملاء', customerPayments: 'دفعات العملاء', customerReturnRequests: 'طلبات مرتجعات العملاء', customerReturnReceipts: 'استلام مرتجعات العملاء', customerCreditNotes: 'إشعارات العملاء الدائنة', directSales: 'البيع المباشر', pointOfSale: 'نقطة البيع', directSalesReturns: 'مرتجعات البيع المباشر', chartOfAccounts: 'شجرة الحسابات',
  },
  'en-US': {
    manualJournals: 'Manual journals',
    financialReports: 'Financial reports',
    agingReports: 'Aging and reconciliation',
    purchasingCenter: 'Purchasing center', purchasingCenterDescription: 'A unified workspace for planning, sourcing, receiving, payables, and supplier operations.', salesCenter: 'Sales center', salesCenterDescription: 'A unified workspace for pricing, orders, delivery, collection, returns, and immediate sales.', accountingCenter: 'Accounting center', accountingCenterDescription: 'Organized access to the chart of accounts, accounting setup, periods, currencies, and branch mappings.', noModuleDestinations: 'No destinations available', noModuleDestinationsDescription: 'You do not have permission to open a workspace in this module.', openWorkspace: 'Open workspace',
    planningAndSourcing: 'Planning and sourcing', receivingAndFulfillment: 'Receiving and fulfillment', payablesAndControl: 'Payables and control', supplierOperations: 'Supplier operations', commercialPreparation: 'Commercial preparation', salesFulfillment: 'Sales fulfillment', receivablesAndCollection: 'Receivables and collection', returnsAndCredits: 'Returns and credits', immediateSales: 'Immediate sales', accountingSetup: 'Accounting setup',
    purchaseApprovalRules: 'Purchase approval rules', purchaseSourcingWorkspace: 'Sourcing and quotations', purchaseOrders: 'Purchase orders', purchaseAgreements: 'Purchase agreements', purchaseTemplates: 'Purchase templates', purchaseReplenishment: 'Purchase replenishment', goodsReceipts: 'Goods receipts', serviceAcceptances: 'Service acceptances', purchaseReturns: 'Receipt returns', landedCosts: 'Landed costs', vendorBills: 'Vendor bills', supplierCreditNotes: 'Supplier credit notes', supplierPayments: 'Supplier payments', purchaseReversals: 'Purchase document reversals', purchaseAccountingReconciliation: 'Purchase accounting reconciliation', supplierTerms: 'Supplier terms', supplierTermExceptions: 'Supplier term exceptions', supplierCommunications: 'Supplier communications', supplierIntegrations: 'Supplier integrations', purchaseAnalytics: 'Purchase analytics', legacyPurchases: 'Direct purchases', legacyPurchaseReturns: 'Direct purchase returns',
    accountingReview: 'Accounting review', journalEntries: 'Journal entries', salesPricing: 'Sales pricing', salesQuotationTemplates: 'Quotation templates', salesQuotations: 'Sales quotations', salesApprovalRules: 'Sales approval rules', salesOrders: 'Sales orders', salesDeliveries: 'Sales deliveries', customerInvoices: 'Customer invoices', customerPayments: 'Customer payments', customerReturnRequests: 'Customer return requests', customerReturnReceipts: 'Customer return receipts', customerCreditNotes: 'Customer credit notes', directSales: 'Direct sales', pointOfSale: 'Point of sale', directSalesReturns: 'Direct sales returns', chartOfAccounts: 'Chart of accounts',
  },
} as const;

export type NavigationTextKey = MessageKey | keyof typeof navigationMessages['en-US'];

@Injectable({ providedIn: 'root' })
export class NavigationLocalizationService {
  private readonly i18n = inject(LocalizationService);

  text(key: NavigationTextKey): string {
    const locale = this.i18n.locale() === 'ar-JO' ? 'ar-JO' : 'en-US';
    return (navigationMessages[locale] as Partial<Record<NavigationTextKey, string>>)[key]
      ?? this.i18n.text(key as MessageKey);
  }
}
