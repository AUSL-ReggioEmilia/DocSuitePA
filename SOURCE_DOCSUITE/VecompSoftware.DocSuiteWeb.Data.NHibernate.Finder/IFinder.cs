using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace VecompSoftware.DocSuiteWeb.Data.NHibernate.Finder
{
    public interface IFinder
    {
        /// <summary>
        /// Paginazione
        /// </summary>
        int PageIndex { get; set; }
        /// <summary>
        /// La proprità abilita la paginazione dei risultati nella griglia.
        /// </summary>
        bool EnablePaging { get; set; }
        /// <summary>
        /// Numero di elementi nella pagina
        /// </summary>
        int PageSize { get; set; }
        /// <summary>
        /// Indice della pagina che sti sta visualizzando
        /// </summary>
        int CustomPageIndex { get; set; }
        /// <summary>
        /// Abilita il join delle tabelle
        /// </summary>
        bool EnableTableJoin { get; set; }
        /// <summary>
        /// Abilita il fetch
        /// </summary>
        bool EnableFetchMode { get; set; }

        /// <summary>
        /// Conteggio risultati
        /// </summary>
        /// <returns></returns>
        int Count();

        /// <summary>
        /// Pulizia delle sort expressions
        /// </summary>
        void SortExpressionsClear();
        /// <summary>
        /// Pulizia delle filter expressions
        /// </summary>
        void FilterExpressionsClear();
    }
    
    /// <summary>
    /// Interfaccia per un generico finder
    /// </summary>
    /// <typeparam name="T">Oggetto del finder</typeparam>
    public interface IFinder<T, THeader> : IFinder
    {
        ICollection<THeader> DoSearchHeader();
    
        ICollection<T> DoSearch();

        /// <summary>
        /// Ricerca con ordinamento
        /// </summary>
        /// <param name="sortExpr"></param>
        /// <returns></returns>
        ICollection<T> DoSearch(ICollection<SortExpression<T>> sortExpr);

        /// <summary>
        /// Ricerca con ordinamento e paginazione
        /// </summary>
        /// <param name="sortExpression">Espressione utilizzata per il filtro</param>
        /// <param name="startRow">Riga iniziale della pagina</param>
        /// <param name="pageSize">Dimensione della pagina</param>
        /// <returns></returns>
        ICollection<T> DoSearch(ICollection<SortExpression<T>> sortExpression, int startRow, int pageSize);

        /// <summary>
        /// Ordinamento
        /// </summary>
        ICollection<SortExpression<T>> SortExpressions { get; set; }

        /// <summary>
        /// Espressione da utilizzare per filtrare i dati
        /// </summary>
        ICollection<Expression<Func<T, bool>>> FilterExpressions { get; set; }    
    }    
}
