using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.DTO.UDS;

namespace VecompSoftware.DocSuiteWeb.Facade.Common.UDS
{
    public class UDSTreeViewFacade
    {
        #region [ Fields ]

        private ICollection<UDSTreeViewDto> _dtosInserted;
        #endregion

        #region [ Constructor ]

        public UDSTreeViewFacade()
        {
            this._dtosInserted = new List<UDSTreeViewDto>();
        }
        #endregion

        #region [ Methods ]

        #region [ Category ]

        public ICollection<UDSTreeViewDto> GetCategoryTreeView(ICollection<Category> categories)
        {
            UDSTreeViewState state = new UDSTreeViewState()
            {
                Checked = false,
                Disabled = false,
                Opened = false,
                Selected = false
            };
            return GetCategoryTreeView(categories, state);
        }

        public ICollection<UDSTreeViewDto> GetCategoryTreeView(ICollection<Category> categories, UDSTreeViewState state)
        {
            return GetCategoryTreeView(categories, state, true);
        }

        public ICollection<UDSTreeViewDto> GetCategoryTreeView(ICollection<Category> categories, UDSTreeViewState state, bool setChildren)
        {
            foreach (Category category in categories)
            {
                UDSTreeViewDto result = AddCategoryNode(category, state, setChildren);
                UpdateSource(this._dtosInserted, result);
            }

            return this._dtosInserted;
        }

        private UDSTreeViewDto AddCategoryNode(Category category, UDSTreeViewState state, bool setChildren)
        {
            UDSTreeViewDto node = null;
            if (category.Parent != null)
            {
                node = GetParentNode(this._dtosInserted, category.Parent.Id);
                node = node ?? AddCategoryNode(category.Parent, state, setChildren);
            }

            UDSTreeViewDto newNode = new UDSTreeViewDto()
            {
                Id = category.Id,
                Text = category.GetFullName(),
                State = state,
                Children = setChildren && category.HasChildren
            };

            return node == null ? newNode : AddChildrenNode(node, category.Parent.Id, newNode);
        }

        #endregion

        #region [ Container ]

        public ICollection<UDSTreeViewDto> GetContainerTreeView(IList<Container> containers)
        {
            UDSTreeViewState state = new UDSTreeViewState()
            {
                Checked = false,
                Disabled = false,
                Opened = false,
                Selected = false
            };
            return GetContainerTreeView(containers, state);
        }

        public ICollection<UDSTreeViewDto> GetContainerTreeView(IList<Container> containers, UDSTreeViewState state)
        {
            return GetContainerTreeView(containers, state, true);
        }

        public ICollection<UDSTreeViewDto> GetContainerTreeView(IList<Container> containers, UDSTreeViewState state, bool setChildren)
        {
            return containers.Select((s) => new UDSTreeViewDto()
            {
                Id = s.Id,
                Text = s.Name,
                State = state
            }).ToList();
        }

        #endregion

        private UDSTreeViewDto GetParentNode(ICollection<UDSTreeViewDto> parentDtos, int idParent)
        {
            foreach (UDSTreeViewDto dto in parentDtos)
            {
                if (dto.Id.Equals(idParent))
                    return dto;

                UDSTreeViewDto parent = null;
                if (dto.HasChildren())
                    parent = GetParentNode((ICollection<UDSTreeViewDto>)dto.Children, idParent);

                if (parent != null)
                    return parent;
            }
            return null;
        }

        private void UpdateSource(ICollection<UDSTreeViewDto> dtos, UDSTreeViewDto dtoToUpdate)
        {
            UDSTreeViewDto element = GetParentNode(dtos, dtoToUpdate.Id.Value);
            if (element == null)
            {
                dtos.Add(dtoToUpdate);
            }    
            else
            {
                if (dtoToUpdate.HasChildren())
                {
                    foreach (UDSTreeViewDto child in (ICollection<UDSTreeViewDto>)dtoToUpdate.Children)
                    {
                        UpdateSource(dtos, child);
                    }
                }                    
            }           
        }

        private UDSTreeViewDto AddChildrenNode(UDSTreeViewDto parentDto, int parentid, UDSTreeViewDto newDtos)
        {
            UDSTreeViewDto parent = GetParentNode(new List<UDSTreeViewDto>() { parentDto }, parentid);
            if (parent == null)
                return null;

            if (!parent.HasChildren())
            {
                parent.Children = new List<UDSTreeViewDto>();
            }

            //Verifico se l'elemento è già presente come child
            ICollection<UDSTreeViewDto> childrenDtos = (ICollection<UDSTreeViewDto>)parent.Children;
            if (!childrenDtos.Any(x => x.Id.Equals(newDtos.Id)))
            {
                childrenDtos.Add(newDtos);
            }

            return parentDto;
        }
        #endregion
    }
}
