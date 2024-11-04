# Pantry Chef 2
*Copy of my [Hackathon submission](https://github.com/Spidious/Pantry-Chef) for me to continue working on.*

### Launching the App
Download the the files in this repository and launch the `start.bat` script.

### Using the App
The recipies tab is populated with all recipies you can make given the items in your pantry. Click a recipe to open it. 
You can add a recipe in by clicking the `+` button next to the search bar.
You can also display all entered recipes with the `Show All Recipes` button

The Pantry tab will show items in your pantry. You can remove items by double clicking them. 
Add items by double clicking them in the ingredients tab or using a comma separated list in the `+` menu.

The ingredients tab shows all ingredients in the database. More ingredients can be added by either using the `+` menu or by using a comma separated list in the Recipes add menu.
Any ingredient added in the recipes manu will automatically be added to the global ingredients if it does not exist already.

# About the Project

Ever come home to an empty fridge? Our app helps by turning pantry ingredients into recipe ideas, reducing unnecessary trips and food waste. Save time, money, and make cooking easier!

## Inspiration

As college students having to balance rigorous course work, working a job and maintaining a social life can make it incredible difficult to meal plan on a budget.

## What it does

Our project allows users to select ingredients that they already have in their own pantry and input them into our application. Once Pantry Chef has the user list of ingredients it provides a list of possible recipes

## How we built it

The app is built on C#.NET written in Microsoft Visual Studio. Using a SQLite database we were able to store ingredients and recipes and query them efficiently.

## Challenges we ran into

Both of us struggled with C# and XAML in the beginning but by the end we were quickly and effectively making changes and additions. Additionally, in working around compatibility issues with Microsoft and Apple products, we had to figure out how to use git on a device where we did not have the ability to install git.

## Accomplishments that we're proud of

A few of our SQL queries felt rather intense in figuring out the solution. Additionally, the images displayed on the recipes tab are not stored locally, rather they fetched from the internet.

## What we learned

We learned how to integrate SQLite with C# while using the various tools and features of Visual Studio that can often seem intimidating.

## What's next for Pantry Chef

In the future we would love to see the input of a recipe being the submission of a URL whose contents can be retrieved and inserted to the database quickly and efficiently. We would also like to have a better user experience.
