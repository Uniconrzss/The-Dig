using System;
using System.IO;

//Created By Uni
//Discord: unicornzss

class TheDig
{
    static void Main() 
    {
        double score = 0;
        double scoreLost = 0;
        Random rand = new Random();
        int level = 1;
        int lvl1Blocks = 4;
        int blocksLeft = 10;
        int mapSizeY = 12;
        Prop[] walls = new Prop[12];
        int mapSizeX = 20;
        Player player = new Player(10, 10, 5, 5);
        Console.CursorVisible = false;
        bool drew = false;
        bool debug = false;
        Menus menu = new Menus();
        char playerTexture = Convert.ToChar(8857);
        string[] foundRecipies = new string[4];
        Console.CursorVisible = false;
        menu.Help();
        
        levels:
        walls = BuildWalls((lvl1Blocks/2)*level, mapSizeX, mapSizeY, lvl1Blocks, level);
        blocksLeft = (lvl1Blocks/2)*level;
        
        bool inGame = true;
        while (inGame)
        {
            //Check how many walls left
            blocksLeft = walls.Length;
            for (int i = 0; i < walls.Length; i++)
            {
                walls[i].CheckStrength();
                if (walls[i].destroyed)
                {
                    blocksLeft--;
                }
            }

            player.pickaxe.CalculateDamage();
            if (blocksLeft == 0)
            {
                level++;
                inGame = false;
                goto levels;
            }
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.Clear();
            Console.WriteLine($"LVL:{level}   LEFT:{blocksLeft}   PICK:{player.pickaxe.rarity} {player.pickaxe.name} {player.pickaxe.damage}");
            
            //Renderer
            for (int y = 0; y < mapSizeY; y++)
            {
                Console.BackgroundColor = ConsoleColor.DarkGray;
                for (int x = 0; x < mapSizeX; x++)
                {
                    drew = false;
                    if (y == 0 || y == mapSizeY-1)
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write("*");
                        Console.ForegroundColor = ConsoleColor.White;
                        drew = true;
                    }
                    if (x == 0 || x == mapSizeX-1)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write(" *");
                        Console.ForegroundColor = ConsoleColor.White;
                        drew = true;
                    }
                    //Render walls
                    foreach (Prop wall in walls)
                    {
                        if (wall.x == x && wall.y == y && wall.destroyed == false)
                        {
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.Write(wall.texture);
                            Console.ForegroundColor = ConsoleColor.White;
                            drew = true;
                        }
                    }
                    
                    if (!drew)
                    {
                        //Render Player
                        if (player.x == x && player.y == y)
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write(playerTexture);
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else
                        {
                            Console.Write(" ");
                        }
                    }
                }
                Console.WriteLine();
            }
            Console.BackgroundColor = ConsoleColor.DarkYellow;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write($"Your XP: {player.xp}");
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.Write("   ");
            Console.BackgroundColor = ConsoleColor.DarkYellow;
            Console.Write($"Score: {Math.Round(score,2)}");
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.Write("   ");
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"Score Lost: {Math.Round(scoreLost,2)}\n");
            Console.BackgroundColor = ConsoleColor.DarkGray;
            if (debug == true)
            {
                Console.WriteLine($"X:{player.x}   Y:{player.y}");
                Console.WriteLine("------TARGET------");
                foreach (Prop wall in walls)
                {
                    if (!wall.destroyed)
                    {
                        Console.WriteLine($"X:{wall.x}   Y:{wall.y} HP:{wall.strength}");
                    }
                }
                Console.WriteLine("------Found Recipies------");
                for (int i = 0; i < foundRecipies.Length; i++)
                {
                    Console.WriteLine(foundRecipies[i]);
                }
            }
            else
            {
                Console.WriteLine("==========Inventory==========");
                Console.WriteLine($"Iron: {player.inventory.iron}");
                Console.WriteLine($"Gold: {player.inventory.gold}");
                Console.WriteLine($"Quartz: {player.inventory.quartz}");
                Console.WriteLine($"Mythril: {player.inventory.mythril}");
                Console.WriteLine("==========Press H for Help==========");
            }
            ConsoleKeyInfo input = Console.ReadKey();
            
            switch(input.Key)
            {
                case ConsoleKey.W:
                    player.Move("up", mapSizeX, mapSizeY, walls);
                    score-=0.35;
                    scoreLost+=0.35;
                    break;
                case ConsoleKey.S:
                    player.Move("down", mapSizeX, mapSizeY, walls);
                    score-=0.35;
                    scoreLost+=0.35;
                    break;
                case ConsoleKey.A:
                    player.Move("left", mapSizeX, mapSizeY, walls);
                    score-=0.35;
                    scoreLost+=0.35;
                    break;
                case ConsoleKey.D:
                    player.Move("right", mapSizeX, mapSizeY, walls);
                    score-=0.35;
                    scoreLost+=0.35;
                    break;
                case ConsoleKey.Spacebar:
                    bool destroyed = player.Dig(walls, mapSizeX, mapSizeY);
                    if (destroyed)
                    {
                        //Generate reward
                        blocksLeft--;
                        int generate = rand.Next(0, level);
                        if (generate > 12)
                        {
                            player.inventory.mythril++;
                            player.xp += 100;
                            score += 8000+(player.xp*2);
                        }
                        else if (generate > 8)
                        {
                            player.inventory.quartz++;
                            player.xp += 50;
                            score += 500;
                        }
                        else if (generate > 5)
                        {
                            player.inventory.gold++;
                            player.xp += 20;
                            score += 250;
                        }
                        else if (generate > 1)
                        {
                            player.inventory.iron++;
                            player.xp += 10;
                            score += 100;
                        }
                        else
                        {
                            player.xp++;
                            score += 10;
                        }
                        generate = rand.Next(0+player.xp,500+player.xp);
                        if (generate > 250)
                        {
                            bool alreadyFound = false;
                            string gen = GenerateRecipie(level);
                            for (int i = 0; i < foundRecipies.Length; i++)
                            {
                                if (foundRecipies[i] == gen)
                                {
                                    alreadyFound = true;
                                }
                            }
                            if (!alreadyFound)
                            {
                                Console.Clear();
                                Console.BackgroundColor = ConsoleColor.DarkYellow;
                                Console.WriteLine($"You found a recipie for a {gen}!");
                                Console.BackgroundColor = ConsoleColor.DarkGray;
                                Console.WriteLine("The recipie is:");
                                switch (gen)
                                {
                                    case "Mythril Pickaxe":
                                        Console.Write("7x - Mythril\n20x - Quartz\n30x - Gold\n10x - Iron\n");
                                        break;
                                    case "Quartz Pickaxe":
                                        Console.Write("10x - Quartz\n10x - Gold\n35x - Iron\n");
                                        break;
                                    case "Gold Pickaxe":
                                        Console.Write("15x - Gold\n20x - Iron\n");
                                        break;
                                    case "Iron Pickaxe":
                                        Console.Write("20x - Iron\n");
                                        break;
                                }
                                Console.WriteLine("You can craft this weapon by opening the crafting menu! [E]");
                                Console.WriteLine("Or enchant your current one by opening the enchanting menu! [R]");
                                Console.WriteLine("Press Enter to continue!");
                                Console.ReadLine();
                                for (int i = 0; i < foundRecipies.Length; i++)
                                {
                                    if (foundRecipies[i] == null)
                                    {
                                        foundRecipies[i] = gen;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    break;
                case ConsoleKey.F1: //debug
                    if (debug == false)
                    {
                        debug = true;
                    }
                    else
                    {
                        debug = false;
                    }
                    break;
                case ConsoleKey.E: //Crafting Menu
                    menu.Crafting(player);
                    break;
                case ConsoleKey.R: //Enchanting Menu
                    menu.Enchanting(player);
                    break;
                case ConsoleKey.H:
                    menu.Help();
                    break;
                case ConsoleKey.F2:
                    string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                    string strWorkPath = System.IO.Path.GetDirectoryName(strExeFilePath);
                    string saveInfo = "==============The Dig Save File==============\nLevel Reached: " + level + "\nScore: " + score + "\nScore Lost: " + scoreLost + "\nXP Gotten: " + player.xp + "\nPickaxe: " + player.pickaxe.rarity + " " + player.pickaxe.name + " [" + player.pickaxe.damage + "]\nResources:\nMythril - " + player.inventory.mythril + "\nQuartz - " + player.inventory.quartz + "\nGold - " + player.inventory.gold + "\nIron - " + player.inventory.iron;
                    File.WriteAllText(strWorkPath+"/DigSave.txt", saveInfo);
                    Console.Clear();
                    Console.WriteLine("Stats Saved.");
                    Console.ReadKey();
                    break;
                default:
                    break;
            }
        }
    }
    
    static string GenerateRecipie(int level)
    {
        Random rand = new Random();
        int generated = rand.Next(1,level);
        if (generated > 10)
        {
            return "Mythril Pickaxe";
        }
        else if (generated > 8)
        {
            return "Quartz Pickaxe";
        }
        else if (generated > 5)
        {
            return "Gold Pickaxe";
        }
        else if (generated > 0)
        {
            return "Iron Pickaxe";
        }
        else
        {
            return "a";
        }
    }
    //Return an array of randomly generated walls.
    static Prop[] BuildWalls(int ammount, int mapMaxX, int mapMaxY, int lvl1Blocks, int level)
    {
        Prop[] walls = new Prop[ammount];
        Random rand = new Random();
        int x;
        int y;
        
        for (int i = 0; i < walls.Length; i++)
        {
            generatexy:
            x = rand.Next(2, mapMaxX-2);
            y = rand.Next(2, mapMaxY-2);

            for (int j = 0; j < i; j++)
            {
                if (walls[j].x == x && walls[j].y == y)
                {
                    goto generatexy;
                }
            }

            walls[i] = new Prop();
            walls[i].Setup(x, y, "wall", level);
        }
        
        return walls;
    }
}

class Prop
{
    public string type;
    public char texture;
    public int strength;
    public bool destroyed;
    public int x;
    public int y;
    
    public void Setup(int posx, int posy, string typ, int str)
    {
        strength = str;
        type = typ;
        x = posx;
        y = posy;
        SetTexture();
    }
    
    public void CheckStrength()
    {
        if (strength < 1)
        {
            destroyed = true;
        }
    }
    
    public void SetTexture()
    {
       switch (type)
        {
            case "wall":
                texture = Convert.ToChar(8864);
                break;
            default:
                texture = 'F';
                break;
        } 
    }
}

class Player
{
    public int hp;
    public int stamina;
    public int x;
    public int y;
    public string lastMoved;
    public Pickaxe pickaxe;
    public Inventory inventory;
    public int xp;
    
    public Player(int starthealth, int startstamina, int posx, int posy)
    {
        hp = starthealth;
        stamina = startstamina;
        x = posx;
        y = posy;
        pickaxe = new Pickaxe();
        pickaxe.name = "Stone";
        pickaxe.rarity = "Basic";
        inventory = new Inventory();
    }
    
    public void Move(string where, int maxMapX, int maxMapY, Prop[] mapBlocks)
    {
        Prop tempProp;
        switch (where)
        {
            case "up":
                tempProp = Check(x, y-1, maxMapX, maxMapY, mapBlocks);
                if (tempProp.destroyed)
                {
                    y--;
                    lastMoved = "up";
                }
                break;
            case "down":
                tempProp = Check(x, y+1, maxMapX, maxMapY, mapBlocks);
                if (tempProp.destroyed)
                {
                    y++;
                    lastMoved = "down";
                }
                break;
            case "left":
                tempProp = Check(x-1, y, maxMapX, maxMapY, mapBlocks);
                if (tempProp.destroyed)
                {
                    x--;
                    lastMoved = "left";
                }
                break;
            case "right":
                tempProp = Check(x+1, y, maxMapX, maxMapY, mapBlocks);
                if (tempProp.destroyed)
                {
                    x++;
                    lastMoved = "right";
                }
                break;
        }
    }
    
    static Prop Check(int tx, int ty, int maxMapX, int maxMapY, Prop[] blocks)
    {
        bool checkReturn = false;
        Prop propReturn = new Prop();
        for (int i = 0; i < blocks.Length; i++)
        {
            if (blocks[i].x == tx && blocks[i].y == ty)
            {
                checkReturn = true;
                propReturn = blocks[i];
            }
        }
        if (tx == 0 || tx == maxMapX-1 || ty == 0 || ty == maxMapY-1)
        {
            checkReturn = true;
            propReturn = new Prop();
            propReturn.strength = 4;
        }
        if (!checkReturn)
        {
            propReturn = new Prop();
            propReturn.strength = 0;
            propReturn.destroyed = true;
        }
        
        return propReturn;
    }
    
    public bool Dig(Prop[] mapBlocks, int maxMapX, int maxMapY)
    {
        Prop tgtProp = new Prop();
        bool destroyed = false;
        
        switch (lastMoved)
        {
            case "up":
                tgtProp = Check(x, y-1, maxMapX, maxMapY, mapBlocks);
                break;
            case "down":
                tgtProp = Check(x, y+1, maxMapX, maxMapY, mapBlocks);
                break;
            case "left":
                tgtProp = Check(x-1, y, maxMapX, maxMapY, mapBlocks);
                break;
            case "right":
                tgtProp = Check(x+1, y, maxMapX, maxMapY, mapBlocks);
                break;
        }
        if (tgtProp.strength > 0)
        {
            tgtProp.strength = tgtProp.strength-pickaxe.damage; //Deal damage to Prop
            tgtProp.CheckStrength();
            if (tgtProp.strength <= 0)
            {
                destroyed = true;
            }
        }
        return destroyed;
    }
}

class Pickaxe
{
    public string name;
    public string rarity;
    public int damage;

    public void CalculateDamage()
    {
        damage = 0;
        switch(name)
        {
            case "Stone":
                damage++;
                break;
            case "Iron":
                damage+=2;
                break;
            case "Gold":
                damage+=3;
                break;
            case "Quartz":
                damage+=4;
                break;
            case "Mythril":
                damage+=5;
                break;
        }

        switch(rarity)
        {
            case "Basic":
                break;
            case "Uncommon":
                damage++;
                break;
            case "Rare":
                damage+=2;
                break;
            case "Mythic":
                damage+=3;
                break;
            case "Godly":
                damage += 4+(damage*2);
                break;
        }
    }
}

class Inventory
{
    public int iron = 0;
    public int gold = 0;
    public int quartz = 0;
    public int mythril = 0;
}

class Menus
{
    public void Crafting(Player player)
    {
        int selection = 0;
        int selectedMythril = 0;
        int selectedQuartz = 0;
        int selectedGold = 0;
        int selectedIron = 0;
        ConsoleKeyInfo input;
        bool inLoop = true;

        while (inLoop)
        {
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Clear();
            Console.WriteLine("=============Crafting=============");
            Console.WriteLine("Warning: Crafting an item deletes stuff from your inventory, even if the crafting doesnt succeed!");
            Console.WriteLine("Use the arrow keys and enter to navigate the menu!");
            switch (selection)
            {
                case 0:
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(Convert.ToChar(10158)+$"Mythril - {selectedMythril}x");
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"Quartz - {selectedQuartz}x");
                    Console.WriteLine($"Gold - {selectedGold}x");
                    Console.WriteLine($"Iron - {selectedIron}x");
                    Console.WriteLine("Craft");
                    Console.WriteLine("Exit");
                    break;
                case 1:
                    Console.WriteLine($"Mythril - {selectedMythril}x");
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine(Convert.ToChar(10158)+$"Quartz - {selectedQuartz}x");
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"Gold - {selectedGold}x");
                    Console.WriteLine($"Iron - {selectedIron}x");
                    Console.WriteLine("Craft");
                    Console.WriteLine("Exit");
                    break;
                case 2:
                    Console.WriteLine($"Mythril - {selectedMythril}x");
                    Console.WriteLine($"Quartz - {selectedQuartz}x");
                    Console.BackgroundColor = ConsoleColor.DarkYellow;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine(Convert.ToChar(10158)+$"Gold - {selectedGold}x");
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"Iron - {selectedIron}x");
                    Console.WriteLine("Craft");
                    Console.WriteLine("Exit");
                    break;
                case 3:
                    Console.WriteLine($"Mythril - {selectedMythril}x");
                    Console.WriteLine($"Quartz - {selectedQuartz}x");
                    Console.WriteLine($"Gold - {selectedGold}x");
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine(Convert.ToChar(10158)+$"Iron - {selectedIron}x");
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("Craft");
                    Console.WriteLine("Exit");
                    break;
                case 4:
                    Console.WriteLine($"Mythril - {selectedMythril}x");
                    Console.WriteLine($"Quartz - {selectedQuartz}x");
                    Console.WriteLine($"Gold - {selectedGold}x");
                    Console.WriteLine($"Iron - {selectedIron}x");
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine(Convert.ToChar(10158)+"Craft");
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("Exit");
                    break;
                case 5:
                    Console.WriteLine($"Mythril - {selectedMythril}x");
                    Console.WriteLine($"Quartz - {selectedQuartz}x");
                    Console.WriteLine($"Gold - {selectedGold}x");
                    Console.WriteLine($"Iron - {selectedIron}x");
                    Console.WriteLine("Craft");
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(Convert.ToChar(10158)+"Exit");
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
            }
            input = Console.ReadKey();
            switch (input.Key)
            {
                case ConsoleKey.UpArrow:
                    if (selection > 0)
                    {
                        selection--;
                    }
                    break;
                case ConsoleKey.DownArrow:
                    if (selection < 5)
                    {
                        selection++;
                    }
                    break;
                case ConsoleKey.Enter:
                    if (selection == 4)
                    {
                        if (player.inventory.mythril >= selectedMythril && player.inventory.quartz >= selectedQuartz && player.inventory.gold >= selectedGold && player.inventory.iron >= selectedIron)
                        {
                            if (selectedMythril == 7 && selectedQuartz == 20 && selectedGold == 30 && selectedIron == 10)
                            {
                                player.pickaxe.name = "Mythril";
                                Console.Clear();
                                Console.WriteLine("You have crafted a Mythril Pickaxe!");
                                Console.ReadKey();
                            }
                            else if (selectedQuartz == 10 && selectedGold == 10 && selectedIron == 35)
                            {
                                player.pickaxe.name = "Quartz";
                                Console.Clear();
                                Console.WriteLine("You have crafted a Quartz Pickaxe!");
                                Console.ReadKey();
                            }
                            else if (selectedGold == 15 && selectedIron == 20)
                            {
                                player.pickaxe.name = "Gold";
                                Console.Clear();
                                Console.WriteLine("You have crafted a Gold Pickaxe!");
                                Console.ReadKey();
                            }
                            else if (selectedIron == 20)
                            {

                                player.pickaxe.name = "Iron";
                                Console.Clear();
                                Console.WriteLine("You have crafted an Iron Pickaxe!");
                                Console.ReadKey();
                            }
                            player.inventory.mythril -= selectedMythril;
                            player.inventory.quartz -= selectedQuartz;
                            player.inventory.gold -= selectedGold;
                            player.inventory.iron -= selectedIron;
                            player.pickaxe.rarity = "Basic";
                        }
                    }
                    else if (selection == 5)
                    {
                        inLoop = false;
                    }
                    break;
                case ConsoleKey.RightArrow:
                    switch (selection)
                    {
                        case 0:
                            selectedMythril++;
                            break;
                        case 1:
                            selectedQuartz++;
                            break;
                        case 2:
                            selectedGold++;
                            break;
                        case 3:
                            selectedIron++;
                            break;
                    }
                    break;
                case ConsoleKey.LeftArrow:
                    switch (selection)
                    {
                        case 0:
                            if (selectedMythril > 0){selectedMythril--;}
                            break;
                        case 1:
                            if (selectedQuartz > 0) {selectedQuartz--;}
                            break;
                        case 2:
                            if (selectedGold > 0) {selectedGold--;}
                            break;
                        case 3:
                            if (selectedIron > 0) {selectedIron--;}
                            break;
                    }
                    break;
            }
        }
    }
    public void Enchanting(Player player)
    {
        ConsoleKeyInfo selection;
        ConsoleKeyInfo userKeys;
        int selectedXP = 0;
        bool inLoop = true;

        Console.Clear();
        Console.BackgroundColor = ConsoleColor.DarkMagenta;
        Console.WriteLine("=============Enchanting=============");
        Console.BackgroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("1 - Enchant current tool");
        Console.WriteLine("2 - Exit");
        selection = Console.ReadKey();
        if (selection.Key == ConsoleKey.D1)
        {
            while (inLoop)
            {
                Console.Clear();
                Console.BackgroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("=============Enchanting=============");
                Console.WriteLine($"Your XP: {player.xp}\n");
                Console.Write("Exit - Escape\nIncrease ammount - Right arrow\nDecrease ammount - Left arrow\nEnchant - Enter\n\n");
                Console.BackgroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(Convert.ToChar(10158)+$"How much XP do you wish to spend: {selectedXP}");
                userKeys = Console.ReadKey();
                switch (userKeys.Key)
                {
                    case ConsoleKey.RightArrow:
                        if (selectedXP < player.xp)
                        {
                            selectedXP++;
                        }
                        break;
                    case ConsoleKey.LeftArrow:
                        if (selectedXP > 0)
                        {
                            selectedXP--;
                        }
                        break;
                    case ConsoleKey.Escape:
                        inLoop = false;
                        break;
                    case ConsoleKey.Enter:
                        player.pickaxe.rarity = Enchant(selectedXP, player);
                        player.xp -= selectedXP;
                        selectedXP = 0;
                        inLoop = false;
                        break;
                }
            }
        }
    }
    public string Enchant(int exp, Player player)
    {
        Random rand = new Random();
        int generatedAmmount = rand.Next(exp, player.xp*3);

        if (generatedAmmount > 800)
        {
            return "Godly";
        }
        else if (generatedAmmount > 500)
        {
            return "Mythic";
        }
        else if (generatedAmmount > 200)
        {
            return "Rare";
        }
        else if (generatedAmmount > 50)
        {
            return "Uncommon";
        }
        else
        {
            return "Basic";
        }
    }

    public void Help()
    {
        Console.BackgroundColor = ConsoleColor.DarkGray;
        Console.ForegroundColor = ConsoleColor.White;
        Console.Clear();
        Console.BackgroundColor = ConsoleColor.DarkBlue;
        Console.WriteLine("=========================Help Menu=========================");
        Console.BackgroundColor = ConsoleColor.DarkGray;
        Console.WriteLine();
        Console.BackgroundColor = ConsoleColor.DarkBlue;
        Console.Write("The Mission: As a dwarve you must dig yourself to the center of the earth.\nThe deeper you go the more difficult it will get.\nBut dont worry! There are many materials that will help you\non your journey! You can use theese materials to craft new ");
        Console.BackgroundColor = ConsoleColor.DarkGreen;
        Console.Write("Pickaxes.\n");
        Console.BackgroundColor = ConsoleColor.DarkBlue;
        Console.Write(" As a dwarve you also have access to dwarven magic. You can enchant your weapons!\n");
        Console.WriteLine();
        Console.Write("By mining materials you can sometimes get lucky drops. You also gain ");
        Console.BackgroundColor = ConsoleColor.DarkYellow;
        Console.Write("XP");
        Console.BackgroundColor = ConsoleColor.DarkBlue;
        Console.Write(" and ");
        Console.BackgroundColor = ConsoleColor.DarkYellow;
        Console.Write("Score");
        Console.BackgroundColor = ConsoleColor.DarkBlue;
        Console.Write(" by mining, \nbut be careful, you ");
        Console.BackgroundColor = ConsoleColor.DarkRed;
        Console.Write("lose Score");
        Console.BackgroundColor = ConsoleColor.DarkBlue;
        Console.Write(" when moving.");
        Console.BackgroundColor = ConsoleColor.DarkGray;
        Console.Write("\n\n\n");
        Console.WriteLine("=========================Controls=========================");
        Console.WriteLine("W,A,S,D - Movement");
        Console.WriteLine("E - Crafting Menu");
        Console.WriteLine("R - Enchanting Menu");
        Console.WriteLine("H - Help Menu");
        Console.WriteLine("F1 - Debug Info");
        Console.WriteLine("F2 - Save Score\n\n\n");
        Console.WriteLine("Press Enter to continue!");
        Console.ReadLine();
    }
}