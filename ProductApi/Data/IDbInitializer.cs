﻿namespace CustomerApi.Data
{
    public interface IDbInitializer
    {
        void Initialize(ProductApiContext context);
    }
}
