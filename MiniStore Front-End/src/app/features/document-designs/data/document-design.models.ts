export interface DocumentDesignSection { sequence:number; sectionType:number; isVisible:boolean; pageBreakBefore:boolean; title:string; titleArabic:string|null; content:string|null; contentArabic:string|null; }
export interface DocumentDesignForm {
  sourceDesignId:number|null; documentType:number; code:string; version:number; name:string; description:string|null; isDefault:boolean;
  defaultLanguage:number; allowLanguageOverride:boolean; pageSize:number; orientation:number; customPageWidthMm:number|null; customPageHeightMm:number|null;
  marginTopMm:number; marginEndMm:number; marginBottomMm:number; marginStartMm:number; primaryColor:string; accentColor:string; textColor:string; tableHeaderTextColor:string;
  fontFamily:number; headerLayout:number; useOrganizationLogo:boolean; logoPlacement:number; logoMaximumHeightPx:number; organizationNamePlacement:number;
  showLegalName:boolean; showTradeName:boolean; showRegistrationNumber:boolean; showTaxNumber:boolean; showAddress:boolean; showContactDetails:boolean; showCustomIdentityFields:boolean;
  showPageNumbers:boolean; showStatusWatermark:boolean; showProductCode:boolean; showUnit:boolean; showLineNotes:boolean; showTaxDetails:boolean; showNetTotal:boolean; showTaxTotal:boolean; showQrCode:boolean; showDocumentBarcode:boolean;
  coverTitle:string|null; coverTitleArabic:string|null; coverSubtitle:string|null; coverSubtitleArabic:string|null; headerText:string|null; headerTextArabic:string|null; footerText:string|null; footerTextArabic:string|null; watermarkText:string|null; watermarkTextArabic:string|null; watermarkOpacity:number;
  sections:DocumentDesignSection[];
}
export interface DocumentDesignSummary { id:number; documentType:number; code:string; version:number; name:string; defaultLanguage:number; pageSize:number; orientation:number; primaryColor:string; sectionCount:number; isDefault:boolean; status:number; rowVersion:string; }
export interface DocumentDesignDetails { id:number; design:DocumentDesignForm; status:number; createdByUserId:string; createdAtUtc:string; activatedAtUtc:string|null; rowVersion:string; }
export interface DocumentDesignFilters { status?:number; documentType?:number; search?:string; }
