# CHANGELOG

## UP UNTIL 01-06-2026

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


### TO-DO
- Make the search bars in the Manufacturers page functional
- Make the search bars in the Products page functional
- Make the search bars in the Categories page functional
- Add a function to add a product via the barcode image [DONE]

//ENTER CHANGES HERE STEFAN

## 16-06-2026

### ADDED
- Employee controller with Index function that returns a list of employees, disable account action and enable account action.
- Employee Index View where there is a list of all accounts and their statuses
- In the Index table the disable button appears if the account is active, if not the enable account button is visible
- Added a Employee button in LoginPartial.cshtml linking it to the Employee controller
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
- Linked shopping cart with user upon registering (after making the cashierOnShift required i had to change
the approach of adding a cart to a user upon registering)
- Linked shopping cart with admin seed (same reason as above)
- Hid the Categories, Manufacturers, Products buttons in Layout.cshtml for admin view only
- Users log in with usernames now, not emails
 

### TO-DO
- Make the search bars in the Manufacturers Index page functional
- Make the search bars in the Products Index page functional
- Make the search bars in the Categories Index page functional
- Make the search bars in the Products Index page functional
- Edit the Register.cshtml to look in theme
- Edit the Views/Employees/Index.cshtml to look in theme
- Edit the Login.cshtml to look in theme
- When clicking on the name of the logged in user a drop down menu appears. Change it to fit into theme