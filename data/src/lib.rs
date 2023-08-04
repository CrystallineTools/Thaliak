use thiserror::Error;

pub mod types;

pub trait DataKey: ToString + Clone {}

pub trait DataType<K>: Identifiable + serde::Serialize + serde::de::DeserializeOwned {
    fn get_key(&self) -> K;
}

pub trait Identifiable {
    fn get_id(&self) -> String;
}

#[derive(Error, Debug)]
pub enum DataLoadError {
    #[error("key not found")]
    KeyNotFound,
    #[error("datastore failure: {0}")]
    DataStoreFailure(String),
}

#[derive(Error, Debug)]
pub enum DataSaveError {
    #[error("serialization failure: {0}")]
    SerializationFailure(#[from] serde_json::Error),
    #[error("datastore failure: {0}")]
    DataStoreFailure(String),
}

#[async_trait::async_trait(?Send)]
pub trait Database {
    async fn load<T: DataType<K>, K: DataKey>(&self, key: K) -> Result<T, DataLoadError>;
    async fn save<T: DataType<K>, K: DataKey>(&mut self, key: K, data: T) -> Result<(), DataSaveError>;
    async fn remove<K: DataKey>(&mut self, key: K);
}

macro_rules! impl_key {
    // for dynamic data
    ($key:ident, $data:ty, $format:expr) => {
        impl crate::DataType<$key> for $data {
            fn get_key(&self) -> $key {
                <$key>::new(&self.get_id())
            }
        }

        #[derive(Clone, Debug, serde::Serialize, serde::Deserialize)]
        pub struct $key(String);
        impl $key {
            pub fn new(id: &str) -> Self {
                Self(format!($format, id))
            }
        }

        impl ToString for $key {
            fn to_string(&self) -> String {
                self.0.to_owned()
            }
        }

        impl crate::DataKey for $key {}
    };
    // for static data that still has a key
    ($key:ident, $format:expr) => {
        #[derive(Clone, Debug, serde::Serialize, serde::Deserialize)]
        pub struct $key(String);
        impl $key {
            pub fn new(id: &str) -> Self {
                Self(format!($format, id))
            }
        }

        impl ToString for $key {
            fn to_string(&self) -> String {
                self.0.to_owned()
            }
        }

        impl crate::DataKey for $key {}
    };
}
pub(crate) use impl_key;

#[cfg(test)]
mod test {
    use super::*;
    use serde::{Serialize, Deserialize};

    struct MemoryDatabase {
        data: std::collections::HashMap<String, String>,
    }

    #[async_trait::async_trait(?Send)]
    impl Database for MemoryDatabase {
        async fn load<T: DataType<K>, K: DataKey>(&self, key: K) -> Result<T, DataLoadError> {
            Ok(serde_json::from_str::<T>(self.data.get(&key.to_string()).ok_or(DataLoadError::KeyNotFound)?)
                .map_err(|e| DataLoadError::DataStoreFailure(e.to_string()))?
            )
        }
    
        async fn save<T: DataType<K>, K: DataKey>(&mut self, key: K, value: T) -> Result<(), DataSaveError> {
            self.data.insert(key.to_string(), serde_json::to_string(&value).map_err(|e| DataSaveError::SerializationFailure(e))?);
            Ok(())
        }
    
        async fn remove<K: DataKey>(&mut self, key: K) {
            self.data.remove(&key.to_string());
        }
    }

    #[derive(Clone, Debug, PartialEq, Eq, Serialize, Deserialize)]
    pub struct TestData {
        pub id: usize,
        pub num: usize,
    }

    impl Identifiable for TestData {
        fn get_id(&self) -> String {
            self.id.to_string()
        }
    }

    impl_key!(TestDataKey, TestData, "test:{}");

    #[tokio::test]
    async fn test_database() {
        // create a database
        let mut db = MemoryDatabase {
            data: std::collections::HashMap::new(),
        };

        // create some data
        let data = TestData {
            id: 69,
            num: 420,
        };

        // save it
        db.save(data.get_key(), data.clone()).await.unwrap();

        // ensure we can load it
        let loaded = db.load::<TestData, TestDataKey>(data.get_key()).await.unwrap();
        assert_eq!(data, loaded);

        // remove it
        db.remove(data.get_key()).await;

        // ensure we can't load it
        let loaded = db.load::<TestData, TestDataKey>(data.get_key()).await;
        assert!(loaded.is_err());
    }
}