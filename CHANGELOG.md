# CHANGELOG


## 18-06-2026

### ADDED
- "MKD" next to Total in cart
- Receipts are now printed
- Added ProductExportDTO to transfer information for a export in excel
- Added an ExportProducts function in ProductService
- Added a function in ProductsController to export the index table to a csv file
- added a popup of the pdf of the receipt when checkout is clicked

### FIXED
- User cannot clear an empty cart
- User cannot generate an empty receipt
- Removed Details from CategoriesController and views
- Only admin can edit/delete categories
- only admin can edit/delete manufaturers
- Receipts button works, admin and manager can see all, cashier only their own

### TO-DO
- Add custom charts that the admin can choose to look at what they want (Only admin)
- Add a table of products where the expiration date is in 2 week window (Admin and manager)
- Add a table of what things are off or in a bundle (All roles)
- Add a End Date text area in Register if the contract is fixed [DONE]
- Add start date, contract type and end date in Employees list [DONE]
- Test adding product to cart [DONE]
- Test printing receipt [DONE]
- Add a remove product from cart option in case of a misinput/make the quantity editable and if user puts 0 it delets
the item from the cart [DONE]
- After clicking checkout, a popup with a pic of the receipt appears on screen [DONE]

## 17-06-2026

### ADDED
- Display names for all attributes in Product Domain Model
- Display names for all attributes in Category Domain Model
- Display names for all attributes in Receipt Domain Model
- Display names for all attributes in Manufacturer Domain Model
- Display names for all attributes in ShoppingCart Domain Model
- Double click edit in Categories Index page
- Fixed dropdown menu when clicking the username in the top right corner
- A new role "Manager"
- Instead of a button to add a product via barcode, now they can double click the barcode column empty cell and enter the full
barcode to add an item as well as add it via scanning


### FIXED
- Products/Index.cshtml to view Manufacturer and Expiration Date of products
- Removed ShoppingCarts button on layout page as it is useless
- Added "MKD" after the price in Products Index
- Removed the need to redirect to a page when deleting a product
- Fixed the Views/Products/Index.cshtml to fit in more
- Fixed script to enable double click edit in Products Index page
- Adjusted the drop-down menu for logging out to be as wide as the username button
- Only Admins can edit the employee list
- Only Admins and Managers can see the employee list
- Disable/Enable account buttons are hidden for everyone except Admin
- An admin user cannot disable their own account
- Admin can only double click to edit items in Products/Index.cshtml
- Cart is now definitely associated with user
- Checkout works now


### TO-DO
- Remove the Edit dependency completely in CategoriesController [DONE]
- Add a 'Scan Item' button to Cart to scan the barcode of an item and add them like that [DONE]
- ShoppingCarts in layout will be replaced with Receipts controller for logged in cashier to see their printed 
receipts or if an admin is there they will see all of them
- Add another role (Manager) that doesn't have all priviledges such as adding items or deleting, but they will
have details, view priviledges to keep count of what items are where [DONE]
- Add custom charts that the admin can choose to look at what they want (Only admin)
- Add a table of products where the expiration date is in 2 week window (Admin and manager)
- Add a table of what things are off or in a bundle (All roles)
- Edit the Register.cshtml to look in theme [DONE]
- Edit the Views/Employees/Index.cshtml to look in theme [DONE]
- Edit the Login.cshtml to look in theme [DONE]
- When clicking on the name of the logged in user a drop down menu appears. Change it to fit into theme [DONE]
- Clean up Views/Categories/Index.cshtml in theme [DONE]

## 16-06-2026

### ADDED
- Made the search bars in the Products page functional using DataTables column filtering
- Made the search bars in the Categories page functional using DataTables column filtering
- Confirmed the Manufacturers page column search bars are fully operational
- Employee controller with Index function that returns a list of employees, disable account action and enable account action
- Employee Index View where there is a list of all accounts and their statuses
- In the Index table the disable button appears if the account is active, if not the enable account button is visible
- Added an Employee button in LoginPartial.cshtml linking it to the Employee controller
- Added a restriction for the Employee controller to be only viewed by admin users
- Added a userId to the Receipt to mark the cashier that printed that receipt

### FIXED
- Added [Required] above every attribute in the Product Domain Model
- All non-nullable attributes in Receipt are forced - domain model
- All non-nullable attributes in Categories are forced - domain model
- Added authorization for admin role in Manufacturers controller for security
- Added authorization for admin role in Receipts controller for security
- Added authorization for admin role in Categories controller for security
- Added authorization for ShoppingCarts controller for security
- Added authorization for Products Controller for security
- Added authorization for Receipts Controller for security
- All non-nullable attributes in Manufacturer are forced - domain model
- All non-nullable attributes in ShoppingCart are forced - domain model
- Linked shopping cart with user upon registering (after making the cashierOnShift required i had to change the approach of adding a cart to a user upon registering)
- Linked shopping cart with admin seed (same reason as above)
- Hid the Categories, Manufacturers, Products buttons in Layout.cshtml for admin view only
- Users log in with usernames now, not emails

### TO-DO
- Edit the Register.cshtml to look in theme
- Edit the Views/Employees/Index.cshtml to look in theme
- Edit the Login.cshtml to look in theme
- When clicking on the name of the logged in user a drop down menu appears. Change it to fit into theme

## 04-06-2026

### ADDED
- Implemented Barcode functionality using the ZXing.Net library
- Enhanced product search to include barcode field
- Added barcode symbol (QR code) generation for new products
- Updated product display to show barcode

## 01-06-2026

### ADDED
- Clear Cart function
- Checkout function
- EndDate calculations
- Register button to only be once person is logged in (so they can add employees)
- In ShoppingCartService added a function for clear cart
- Employees require a cashierID to clear the cart
- Added a function to calculate the end date of a contract based on the start date and duration of the contract
- Scaffolded item for Logout (needed to fix the logout bug)
- Javascript function to toggle search bars under each header column in the Manufacturers page
- A new Relational Database to establish the relationship between the product and receipt (order)
- Added an EmployeeID in the SeedData for the initial admin account
- Added two Tasks in ShoppingCarts Controller for checkout and clearing of cart
- User can now enter a **FULL** barcode and add the item in the cart
- If the product is already in the cart, the quantity will be increased by 1 instead of adding a new line item in the cart

### FIXED
- Fixed logout function
- Duration of contract input area is now fisible once a contract type "FIXED" is selected
- Shopping cart is associated to a user upon creation
- Fixed buggy UI in "Cart"
