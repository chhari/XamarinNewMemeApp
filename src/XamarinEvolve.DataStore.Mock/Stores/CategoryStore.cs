﻿using System.Threading.Tasks;
using XamarinEvolve.DataObjects;
using XamarinEvolve.DataStore.Abstractions;

using XamarinEvolve.DataStore.Mock;
using System.Collections.Generic;

namespace XamarinEvolve.DataStore.Mock
{
    public class CategoryStore : BaseStore<Category>, ICategoryStore
    {
        public override Task<IEnumerable<Category>> GetItemsAsync(bool forceRefresh = false)
        {
           var categories = new []
                {
                    new Category { Name = "English", ShortName="English", Color="#B8E986"},
                    new Category { Name = "Hindi", ShortName="Hindi", Color="#F16EB0"},
                    new Category { Name = "Telugu", ShortName="Telugu", Color="#7DD5C9" },
                    new Category { Name = "Tamil", ShortName="Tamil", Color="#51C7E3"},
                    new Category { Name = "Marathi", ShortName="Marathi", Color="#F88F73" },
                    new Category { Name = "Test", ShortName="Test", Color="#4B637E"},
                    new Category { Name = "Monitor", ShortName="Monitor", Color="#AC9AD3" },
                };
            return Task.FromResult(categories as IEnumerable<Category>);
        }
    }
}
