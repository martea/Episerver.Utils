public class EpiserverRemovalHelper{
/// <summary>
        /// Method for removing that inheirits a block/reference that is assignable to any of the types
        /// </summary>
        /// <returns></returns>
        public void ClearUnusedBlocks()
        {
            List<Type> blocksToRemove = GetTypesThatIsAssignableFromType(typeof(IRemovableBlock));
            List<Type> pagesToRemove = GetTypesThatIsAssignableFromType(typeof(IRemovablePage));

            blocksToRemove.ForEach(RemoveBlockType);
            pagesToRemove.ForEach(RemoveBlockType);
        }

        /// <summary>
        /// Method to collect types that are assignable from that type that you provide
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <returns></returns>
        private static List<Type> GetTypesThatIsAssignableFromType(Type interfaceType)
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).Where(x => interfaceType.IsAssignableFrom(x) && !x.IsEquivalentTo(interfaceType)).ToList();
        }
        /// <summary>
        /// Method for collecting usages of blocks and types, and then removes the references that are found.
        /// </summary>
        /// <param name="removableType"></param>
        private void RemoveBlockType(Type removableType)
        {
            var contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
            var contentTypeRepository = ServiceLocator.Current.GetInstance<IContentTypeRepository>();
            var contentModelUsage = ServiceLocator.Current.GetInstance<IContentModelUsage>();
            var contentTypeRepositoryTye = contentTypeRepository.GetType();
            var methods = contentTypeRepositoryTye.GetMethods().Where(x => x.Name == "Load");
            ContentType blockType = contentTypeRepositoryTye.GetMethod("Load", new[] { typeof(Type) }).Invoke(contentTypeRepository, new[] { removableType }) as ContentType;

            if (blockType == null)
            {
                throw new NotSupportedException($"type not supported,{removableType.FullName}");
            }

            var blockTypeUsages = contentModelUsage.ListContentOfContentType(blockType);

            //blockTypeUsages.Select(x => x.ContentLink).ToList().ForEach(x => contentRepository.Delete(x, true));
        }
        }
