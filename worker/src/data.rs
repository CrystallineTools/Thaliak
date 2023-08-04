use thaliak_data::{Database, DataKey, DataType};
use worker::kv::KvStore;

pub use thaliak_data::*;

pub struct KvDatabase(KvStore);
#[async_trait::async_trait(?Send)]
impl Database for KvDatabase {
    async fn load<T: DataType<K>, K: DataKey>(&self, key: K) -> Result<T, DataLoadError> {
        Ok(self.0.get(&key.to_string()).json::<T>().await
            .map_err(|e| DataLoadError::DataStoreFailure(e.to_string()))?
            .ok_or(DataLoadError::KeyNotFound)?
        )
    }

    async fn save<T: DataType<K>, K: DataKey>(&mut self, key: K, value: T) -> Result<(), DataSaveError> {
        self.0.put(&key.to_string(), serde_json::to_string(&value).map_err(|e| DataSaveError::SerializationFailure(e))?)
            .unwrap()
            .execute().await
            .map_err(|e| DataSaveError::DataStoreFailure(e.to_string()))?;

        Ok(())
    }

    async fn remove<K: DataKey>(&mut self, key: K) {
        self.0.delete(&key.to_string()).await.unwrap();
    }
}
