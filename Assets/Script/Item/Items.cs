﻿using System.Collections.Generic;
using System;

public static class Items
{

    static Item basket = new Basket();
    static Item food = new Food();
    static Item hammer = new Hammer();
    static Item hat = new Hat();
    static Item stick = new Stick();
    static Item wood = new Wood();

    static Dictionary<string, Item> items = new Dictionary<string, Item>
    {
        {basket.Tag, basket},
        {food.Tag, food},
        {hammer.Tag, hammer},
        {hat.Tag, hat},
        {stick.Tag, stick},
        {wood.Tag, wood}
    };

    public static Item getItemByTag(string tag)
    {
        if (!items.ContainsKey(tag))
            return null;

        return items[tag];
    }
}
