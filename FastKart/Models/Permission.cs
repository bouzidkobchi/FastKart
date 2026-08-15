namespace FastKart.Models
{
    public enum Permission : byte
    {
        // Users
        UsersIndex = 0,
        UsersCreate = 1,
        UsersEdit = 2,
        UsersDestroy = 3,

        // Roles
        RolesIndex = 4,
        RolesCreate = 5,
        RolesEdit = 6,
        RolesDestroy = 7,

        // Products
        ProductsIndex = 8,
        ProductsCreate = 9,
        ProductsEdit = 10,
        ProductsDestroy = 11,

        // Attributes
        AttributesIndex = 12,
        AttributesCreate = 13,
        AttributesEdit = 14,
        AttributesDestroy = 15,

        // Categories
        CategoriesIndex = 16,
        CategoriesCreate = 17,
        CategoriesEdit = 18,
        CategoriesDestroy = 19,

        // Tags
        TagsIndex = 20,
        TagsCreate = 21,
        TagsEdit = 22,
        TagsDestroy = 23,

        // Stores
        StoresIndex = 24,
        StoresCreate = 25,
        StoresEdit = 26,
        StoresDestroy = 27,

        // Vendor Wallets
        VendorWalletsIndex = 28,
        VendorWalletsCredit = 29,
        VendorWalletsDebit = 30,

        // Commission Histories
        CommissionHistoriesIndex = 31,

        // Withdraw Requests
        WithdrawRequestsIndex = 32,
        WithdrawRequestsCreate = 33,
        WithdrawRequestsAction = 34,

        // Orders
        OrdersIndex = 35,
        OrdersCreate = 36,
        OrdersEdit = 37,

        // Attachments
        AttachmentsIndex = 38,
        AttachmentsCreate = 39,
        AttachmentsDestroy = 40,

        // Blogs
        BlogsIndex = 41,
        BlogsCreate = 42,
        BlogsEdit = 43,
        BlogsDestroy = 44,

        // Pages
        PagesIndex = 45,
        PagesCreate = 46,
        PagesEdit = 47,
        PagesDestroy = 48,

        // Taxes
        TaxesIndex = 49,
        TaxesCreate = 50,
        TaxesEdit = 51,
        TaxesDestroy = 52,

        // Shippings
        ShippingsIndex = 53,
        ShippingsCreate = 54,
        ShippingsEdit = 55,
        ShippingsDestroy = 56,

        // Coupons
        CouponsIndex = 57,
        CouponsCreate = 58,
        CouponsEdit = 59,
        CouponsDestroy = 60,

        // Currencies
        CurrenciesIndex = 61,
        CurrenciesCreate = 62,
        CurrenciesEdit = 63,
        CurrenciesDestroy = 64,

        // Points
        PointsIndex = 65,
        PointsCredit = 66,
        PointsDebit = 67,

        // Wallets
        WalletsIndex = 68,
        WalletsCredit = 69,
        WalletsDebit = 70,

        // Refunds
        RefundsIndex = 71,
        RefundsCreate = 72,
        RefundsAction = 73,

        // Reviews
        ReviewsIndex = 74,
        ReviewsCreate = 75,

        // FAQs
        FaqsIndex = 76,
        FaqsCreate = 77,
        FaqsEdit = 78,
        FaqsDestroy = 79,

        // Themes
        ThemesIndex = 80,
        ThemesEdit = 81,

        // Theme Options
        ThemeOptionsIndex = 82,
        ThemeOptionsEdit = 83,

        // Settings
        SettingsIndex = 84,
        SettingsEdit = 85,

        // Question Answer
        QuestionAnswerIndex = 86,
        QuestionAnswerCreate = 87,
        QuestionAnswerEdit = 88,
        QuestionAnswerDestroy = 89
    }
}