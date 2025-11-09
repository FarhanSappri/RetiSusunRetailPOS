# Point-of-Sale System User Guide

## Overview

The RetiSusun POS system provides a comprehensive solution for retail sales transactions with product management, cart operations, payment processing, and receipt generation.

## Features

### 1. Product Management

#### Accessing Product Management
- After logging in, navigate to the **"Products"** tab
- The tab displays all products in your inventory

#### Product List Columns
- **ID**: Unique product identifier
- **Name**: Product name (automatically converted to Title Case)
- **Brand**: Product brand (e.g., Nestle, Unilever, Gardenia)
- **Category**: Product category (e.g., Food, Beverages, Personal Care)
- **Price (RM)**: Selling price per unit
- **Stock**: Current stock quantity
- **Barcode**: Product barcode for scanning
- **SKU**: Stock Keeping Unit code
- **Unit**: Unit of measurement (pcs, bottle, pack, etc.)

#### Adding a New Product
1. Click **"Add New Product"** button
2. Fill in the product details:
   - Name (required) - will be converted to Title Case
   - Brand (e.g., Nestle, Maggi, Gardenia)
   - Description
   - Category
   - Barcode
   - SKU
   - Cost Price (RM)
   - Selling Price (RM) - this is the price customers pay
   - Stock Quantity
   - Unit (pcs, bottle, pack, etc.)
3. Click **"Add"** to save the product

#### Editing a Product
1. Select a product from the list
2. Click **"Edit Product"** or double-click the product
3. Modify the desired fields
4. Click **"Update"** to save changes

#### Deleting a Product
1. Select a product from the list
2. Click **"Delete Product"**
3. Confirm the deletion
4. Note: This performs a soft delete (product is marked inactive)

### 2. Point-of-Sale (POS) System

#### Accessing the POS
- After logging in, the **"Point of Sale"** tab is the first tab
- The POS interface is divided into three sections:
  - **Available Products** (left)
  - **Shopping Cart** (right top)
  - **Payment & Checkout** (right bottom)

#### Making a Sale

##### Step 1: Select Products
- View all available products in the left panel
- Products show: Name, Brand, Price, and Stock quantity
- **Double-click** a product to add it to the cart
- Or click **"Refresh Products"** to reload the product list

##### Step 2: Manage Cart
The shopping cart displays:
- Product name and brand
- Quantity
- Unit price
- Total price per item

**Cart Operations:**
- **Add to Cart**: Double-click any product in the product list
- **Remove Item**: Select an item and click "Remove Item"
- **Increase Quantity**: Select an item and click "+ Qty"
- **Decrease Quantity**: Select an item and click "- Qty"
- **Clear Cart**: Click "Clear Cart" to remove all items

**Stock Validation:**
- System prevents adding more items than available in stock
- Warning message appears if stock limit is reached

##### Step 3: Configure Tax and Discount

**Tax Rate:**
- Default: 6.00%
- Editable in the "Tax Rate (%)" field
- Tax is automatically calculated based on the subtotal
- Formula: Tax Amount = Subtotal × (Tax Rate ÷ 100)

**Discount:**
- Enter discount amount in RM (Malaysian Ringgit)
- Discount is subtracted from the total
- Can be used for promotions or special offers

**Real-time Calculations:**
The system automatically displays:
- **Subtotal**: Sum of all items in cart
- **Tax**: Calculated based on tax rate
- **Discount**: Discount amount entered
- **TOTAL**: Subtotal + Tax - Discount

##### Step 4: Process Payment

1. **Select Payment Method:**
   - Cash
   - Debit Card
   - Credit Card

2. **Enter Amount Paid:**
   - For cash payments, enter the amount customer pays
   - For card payments, typically equal to total
   - Change is automatically calculated

3. **Review Change:**
   - System displays change amount in blue (if positive) or red (if negative)
   - For cash, ensure amount paid ≥ total

4. **Click "Process Payment":**
   - System validates the transaction
   - Checks for sufficient payment (for cash)
   - Updates inventory (reduces stock quantities)
   - Generates receipt

### 3. Receipt

After successful payment, a receipt is displayed with:

```
========================================
           SALES RECEIPT
========================================
Transaction #: TXN-20251109-0001
Date: 2025-11-09 09:10:53
Cashier: John Doe
========================================
ITEMS:
----------------------------------------
Dove Soap 100g
  2 x RM4.50 = RM9.00
Dutch Lady Milk 1L
  3 x RM8.20 = RM24.60
========================================
Subtotal:        RM33.60
Tax (6.00%):    RM2.02
Discount:       RM0.00
TOTAL:           RM35.62
Payment Method:  Cash
Amount Paid:     RM50.00
Change:          RM14.38
========================================
       Thank you for your business!
========================================
```

**Receipt Information:**
- Unique transaction number
- Date and time of sale
- Cashier name
- Detailed item list with quantities and prices
- Breakdown of subtotal, tax, discount
- Final total amount
- Payment details (method, amount paid, change)

### 4. Sample Products

The system comes with 10 pre-configured products:

1. **Nasi Lemak** (Kak Tie) - RM5.50
2. **Mineral Water 1.5L** (Spritzer) - RM2.00
3. **White Bread** (Gardenia) - RM4.20
4. **Instant Noodles Curry** (Maggi) - RM1.50
5. **Milo Powder 1kg** (Nestle) - RM18.90
6. **Maggi Kari** (Maggi) - RM1.40
7. **Dove Soap 100g** (Unilever) - RM4.50
8. **Pringles Original 107g** (Pringles) - RM6.90
9. **Nescafe Classic 200g** (Nestle) - RM16.50
10. **Dutch Lady Milk 1L** (Dutch Lady) - RM8.20

These products can be edited or deleted, and new products can be added.

## Tips & Best Practices

### For Cashiers
1. Always verify the items in the cart before processing payment
2. Double-check the amount paid for cash transactions
3. Use the refresh button if products don't appear
4. Clear the cart after each transaction

### For Managers/Admins
1. Regularly update product prices and stock levels
2. Review and adjust tax rates as per local regulations
3. Use the discount feature strategically for promotions
4. Monitor stock levels and restock when necessary
5. Keep product information (brands, names) accurate and up-to-date

### Tax Configuration
- The default tax rate is 6% (standard Malaysian SST)
- Tax rate can be adjusted per transaction in the POS interface
- Formula is: Tax = Subtotal × (Rate ÷ 100)
- To change default tax rate, modify the initialization in MainForm.cs

### Discount Management
- Discounts are applied as flat amounts (not percentages)
- To apply percentage discounts, calculate manually:
  - Example: 10% off RM100 = RM10 discount
- Discount is subtracted after tax is applied
- Always document reasons for large discounts

## Troubleshooting

### Product Not Appearing in POS
- Click "Refresh Products" button
- Check if product is active in Product Management
- Verify product is assigned to your business

### Cannot Add More Items to Cart
- Check product stock quantity
- System prevents adding more than available stock
- Restock the product or remove other items

### Payment Processing Fails
- For cash: Ensure amount paid ≥ total
- Check that cart has items
- Verify amount paid is a valid number

### Change Amount Shows Negative
- This indicates insufficient payment
- Increase amount paid or adjust total

## Keyboard Shortcuts

Currently, all operations are mouse-based. Future versions may include:
- F1: Refresh Products
- F2: Focus on Product Search
- Enter: Add Selected Product to Cart
- Delete: Remove Selected Cart Item
- F12: Process Payment

## Support

For issues or questions:
- Review this guide
- Check system logs
- Contact system administrator
- Refer to main README.md for technical details
