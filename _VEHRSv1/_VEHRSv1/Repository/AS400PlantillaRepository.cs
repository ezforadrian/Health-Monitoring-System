using _VEHRSv1.Helper;
using _VEHRSv1.Interface;
using _VEHRSv1.Models;
using _VEHRSv1.Services;
using System.Buffers;
using System.Data.Odbc;
using System.Text.RegularExpressions;

namespace _VEHRSv1.Repository
{
    public class AS400PlantillaRepository : IAS400PlantillaRepository
    {
        private readonly string _configuration;
        private readonly EncryptionService _encrptionService;
        private const string _key = "thequickbrownfox";

        public AS400PlantillaRepository(IConfiguration configuration, EncryptionService encrptionService)
        {
            _configuration = configuration.GetConnectionString("HCMS");
            _encrptionService = encrptionService;
        }
        public async Task<List<vmDepartment>> GetAllDepartments()
        {
            var deptRec = new List<vmDepartment>();

            try
            {
                using (var conn = new OdbcConnection(_configuration))
                {
                    await conn.OpenAsync();

                    using (var cmd = new OdbcCommand(
                        "SELECT distinct substr(a.pmicod, 3, 4) as DepartmentCode, b.pcdes1 as DepartmentDescription " +
                        "FROM S06421A5.PISPROD.pip007 a " +
                        "LEFT OUTER JOIN S06421A5.PISPROD.pip010 b ON substr(a.pmicod, 3, 4) = b.pcval AND b.pccdno = '002' " +
                        "LEFT OUTER JOIN S06421A5.PISPROD.pip010 c ON substr(a.pmicod, 11, 4) = c.pcval AND c.pccdno = '006' " +
                        " WHERE b.pcdes1 <> '' " +
                        "GROUP BY substr(a.pmicod, 3, 4), substr(a.pmicod, 11, 4), b.pcdes1, c.pcdes1" +
                        " ORDER BY b.pcdes1", conn))
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                deptRec.Add(new vmDepartment
                                {
                                    DepartmentCodeEncrypt = _encrptionService.EncryptConstant(_key, reader.GetString(reader.GetOrdinal("DepartmentCode")).Trim()),
                                    DepartmentCode = reader.GetString(reader.GetOrdinal("DepartmentCode")).Trim(),
                                    DepartmentDescription = reader.GetString(reader.GetOrdinal("DepartmentDescription")).Trim(),
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new DatabaseConnectionException("Cannot connect to the AS400 database.", ex);
            }



            return deptRec;
        }

        public async Task<List<vmPosition>> GetAllPositions()
        {
            var postRec = new List<vmPosition>();

            try
            {
                using (var conn = new OdbcConnection(_configuration))
                {
                    await conn.OpenAsync();

                    using (var cmd = new OdbcCommand(
                        "SELECT distinct substr(a.pmicod, 11, 4) as PositionCode, c.pcdes1 as PositionDescription " +
                        "FROM S06421A5.PISPROD.pip007 a " +
                        "LEFT OUTER JOIN S06421A5.PISPROD.pip010 b ON substr(a.pmicod, 3, 4) = b.pcval AND b.pccdno = '002' " +
                        "LEFT OUTER JOIN S06421A5.PISPROD.pip010 c ON substr(a.pmicod, 11, 4) = c.pcval AND c.pccdno = '006' " +
                        " WHERE c.pcdes1 <> '' " +
                        "GROUP BY substr(a.pmicod, 3, 4), substr(a.pmicod, 11, 4), b.pcdes1, c.pcdes1" +
                        " ORDER BY c.pcdes1", conn))
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                postRec.Add(new vmPosition
                                {
                                    PositionCodeEncrypt = _encrptionService.EncryptConstant(_key, reader.GetString(reader.GetOrdinal("PositionCode")).Trim()),
                                    PositionCode = reader.GetString(reader.GetOrdinal("PositionCode")).Trim(),
                                    PositionDescription = reader.GetString(reader.GetOrdinal("PositionDescription")).Trim(),
                                });
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new DatabaseConnectionException("Cannot connect to the AS400 database.", ex);
            }



            return postRec;
        }

        public async Task<vmDepartment> GetDepartmentByCode(string code)
        {
            var deptRec = new vmDepartment();
            try
            {
                using (var conn = new OdbcConnection(_configuration))
                {
                    await conn.OpenAsync();

                    using (var cmd = new OdbcCommand(
                        "SELECT distinct substr(a.pmicod, 3, 4) as DepartmentCode, b.pcdes1 as DepartmentDescription " +
                        "FROM S06421A5.PISPROD.pip007 a " +
                        "LEFT OUTER JOIN S06421A5.PISPROD.pip010 b ON substr(a.pmicod, 3, 4) = b.pcval AND b.pccdno = '002' " +
                        "LEFT OUTER JOIN S06421A5.PISPROD.pip010 c ON substr(a.pmicod, 11, 4) = c.pcval AND c.pccdno = '006' " +
                        "WHERE b.pcdes1 <> '' AND substr(a.pmicod, 3, 4) = ? " +
                        "GROUP BY substr(a.pmicod, 3, 4), substr(a.pmicod, 11, 4), b.pcdes1, c.pcdes1 " +
                        "ORDER BY b.pcdes1", conn))
                    {
                        // Add the parameter value
                        cmd.Parameters.Add(new OdbcParameter("DepartmentCode", code));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                deptRec = new vmDepartment
                                {
                                    DepartmentCodeEncrypt = _encrptionService.EncryptConstant(_key, reader.GetString(reader.GetOrdinal("DepartmentCode")).Trim()),
                                    DepartmentCode = reader.GetString(reader.GetOrdinal("DepartmentCode")).Trim(),
                                    DepartmentDescription = reader.GetString(reader.GetOrdinal("DepartmentDescription")).Trim(),
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new DatabaseConnectionException("Cannot connect to the AS400 database.", ex);
            }


            return deptRec;
        }


        public async Task<vmPosition> GetPositionByCode(string code)
        {
            var postRec = new vmPosition();

            try
            {
                using (var conn = new OdbcConnection(_configuration))
                {
                    await conn.OpenAsync();

                    using (var cmd = new OdbcCommand(
                        "SELECT distinct substr(a.pmicod, 11, 4) as PositionCode, c.pcdes1 as PositionDescription " +
                        "FROM S06421A5.PISPROD.pip007 a " +
                        "LEFT OUTER JOIN S06421A5.PISPROD.pip010 b ON substr(a.pmicod, 3, 4) = b.pcval AND b.pccdno = '002' " +
                        "LEFT OUTER JOIN S06421A5.PISPROD.pip010 c ON substr(a.pmicod, 11, 4) = c.pcval AND c.pccdno = '006' " +
                        " WHERE c.pcdes1 <> '' AND substr(a.pmicod, 11, 4) = ?  " +
                        "GROUP BY substr(a.pmicod, 3, 4), substr(a.pmicod, 11, 4), b.pcdes1, c.pcdes1" +
                        " ORDER BY c.pcdes1", conn))
                    {
                        // Add the parameter value
                        cmd.Parameters.Add(new OdbcParameter("DepartmentCode", code));

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                postRec = new vmPosition
                                {
                                    PositionCodeEncrypt = _encrptionService.EncryptConstant(_key, reader.GetString(reader.GetOrdinal("PositionCode")).Trim()),
                                    PositionCode = reader.GetString(reader.GetOrdinal("PositionCode")).Trim(),
                                    PositionDescription = reader.GetString(reader.GetOrdinal("PositionDescription")).Trim(),
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new DatabaseConnectionException("Cannot connect to the AS400 database.", ex);
            }



            return postRec;
        }

        public string GetDepartmentDescriptionByCode(string code)
        {
            var deptRec = new vmDepartment();

            try
            {
                using (var conn = new OdbcConnection(_configuration))
                {
                    conn.Open();

                    using (var cmd = new OdbcCommand(
                        "SELECT distinct substr(a.pmicod, 3, 4) as DepartmentCode, b.pcdes1 as DepartmentDescription " +
                        "FROM S06421A5.PISPROD.pip007 a " +
                        "LEFT OUTER JOIN S06421A5.PISPROD.pip010 b ON substr(a.pmicod, 3, 4) = b.pcval AND b.pccdno = '002' " +
                        "LEFT OUTER JOIN S06421A5.PISPROD.pip010 c ON substr(a.pmicod, 11, 4) = c.pcval AND c.pccdno = '006' " +
                        "WHERE b.pcdes1 <> '' AND substr(a.pmicod, 3, 4) = ? " +
                        "GROUP BY substr(a.pmicod, 3, 4), substr(a.pmicod, 11, 4), b.pcdes1, c.pcdes1 " +
                        "ORDER BY b.pcdes1", conn))
                    {
                        // Add the parameter value
                        cmd.Parameters.Add(new OdbcParameter("DepartmentCode", code));

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                deptRec = new vmDepartment
                                {
                                    DepartmentCodeEncrypt = _encrptionService.EncryptConstant(_key, reader.GetString(reader.GetOrdinal("DepartmentCode")).Trim()),
                                    DepartmentCode = reader.GetString(reader.GetOrdinal("DepartmentCode")).Trim(),
                                    DepartmentDescription = reader.GetString(reader.GetOrdinal("DepartmentDescription")).Trim(),
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new DatabaseConnectionException("Cannot connect to the AS400 database.", ex);
            }


            return deptRec.DepartmentDescription;
        }

        public string GetPositionDescriptionByCode(string code)
        {
            var postRec = new vmPosition();

            try
            {
                using (var conn = new OdbcConnection(_configuration))
                {
                    conn.OpenAsync();

                    using (var cmd = new OdbcCommand(
                        "SELECT distinct substr(a.pmicod, 11, 4) as PositionCode, c.pcdes1 as PositionDescription " +
                        "FROM S06421A5.PISPROD.pip007 a " +
                        "LEFT OUTER JOIN S06421A5.PISPROD.pip010 b ON substr(a.pmicod, 3, 4) = b.pcval AND b.pccdno = '002' " +
                        "LEFT OUTER JOIN S06421A5.PISPROD.pip010 c ON substr(a.pmicod, 11, 4) = c.pcval AND c.pccdno = '006' " +
                        " WHERE c.pcdes1 <> '' AND substr(a.pmicod, 11, 4) = ?  " +
                        "GROUP BY substr(a.pmicod, 3, 4), substr(a.pmicod, 11, 4), b.pcdes1, c.pcdes1" +
                        " ORDER BY c.pcdes1", conn))
                    {
                        // Add the parameter value
                        cmd.Parameters.Add(new OdbcParameter("DepartmentCode", code));

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                postRec = new vmPosition
                                {
                                    PositionCodeEncrypt = _encrptionService.EncryptConstant(_key, reader.GetString(reader.GetOrdinal("PositionCode")).Trim()),
                                    PositionCode = reader.GetString(reader.GetOrdinal("PositionCode")).Trim(),
                                    PositionDescription = reader.GetString(reader.GetOrdinal("PositionDescription")).Trim(),
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new DatabaseConnectionException("Cannot connect to the AS400 database.", ex);
            }



            return postRec.PositionDescription;
        }

        public async Task<List<vmAS400EmployeeList>> GetEmployeeListAS400()
        {
            var empList = new List<vmAS400EmployeeList>();

            try
            {
                using (var conn = new OdbcConnection(_configuration))
                {
                    await conn.OpenAsync();

                    // Simplified and formatted query
                    string query = @"
                SELECT 'Active' AS Status, a.pmidno, a.pmlnam, a.pmfnam, a.pmmnam, a.pmides, a.pmbdat, a.pmremk,
                    CASE 
                        WHEN SUBSTR(a.pmicod, 1, 2) <> '00' THEN c.pcdes1
                        WHEN SUBSTR(a.pmicod, 3, 4) IN ('0127', '0129') THEN c.pcdes1
                        ELSE b.pcdes1
                    END AS DeptDiv
                FROM S06421A5.PISPROD.pip007 a
                LEFT OUTER JOIN S06421A5.PISPROD.pip010 b 
                    ON SUBSTR(a.pmicod, 3, 4) = b.pcval AND b.pccdno = '002'
                LEFT OUTER JOIN S06421A5.PISPROD.pip010 c 
                    ON SUBSTR(a.pmicod, 3, 6) = c.pcval AND c.pccdno = '003'
                WHERE a.pmstat = 'O'
                
                UNION
                
                SELECT 'Seceded', a.pmidno, a.pmlnam, a.pmfnam, a.pmmnam, a.pmides, a.pmbdat, a.pmremk,
                    CASE 
                        WHEN SUBSTR(a.pmicod, 1, 2) <> '00' THEN c.pcdes1
                        WHEN SUBSTR(a.pmicod, 3, 4) IN ('0127', '0129') THEN c.pcdes1
                        ELSE b.pcdes1
                    END AS DeptDiv
                FROM S06421A5.PISPROD.pip009 a
                LEFT OUTER JOIN S06421A5.PISPROD.pip010 b 
                    ON SUBSTR(a.pmicod, 3, 4) = b.pcval AND b.pccdno = '002'
                LEFT OUTER JOIN S06421A5.PISPROD.pip010 c 
                    ON SUBSTR(a.pmicod, 3, 6) = c.pcval AND c.pccdno = '003'
                WHERE a.pmstat = 'O'";

                    using (var cmd = new OdbcCommand(query, conn))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            empList.Add(new vmAS400EmployeeList
                            {
                                IdNumber = reader["pmidno"].ToString().Trim(),
                                IdEnc = _encrptionService.Encrypt(_key, reader["pmidno"].ToString().Trim()),
                                FullName = $"{reader["pmfnam"].ToString().Trim()} {reader["pmmnam"].ToString().Trim()} {reader["pmlnam"].ToString().Trim()}",
                                LastName = reader["pmlnam"].ToString().Trim(),
                                FirstName = reader["pmfnam"].ToString().Trim(),
                                MiddleName = reader["pmmnam"].ToString().Trim(),
                                Position = reader["pmides"].ToString().Trim(),
                                BirthDate = ConvertToDateOnly(reader["pmbdat"].ToString().Trim())
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new DatabaseConnectionException("Cannot connect to the AS400 database.", ex);
            }

            return empList;
        }






        //public async Task<(List<vmAS400EmployeeList> empList, int totalRecords)> GetEmployeeListAS400(int start, int length, string searchValue)
        //{
        //    var empList = new List<vmAS400EmployeeList>();
        //    int totalRecords = 0; // Variable to hold the total record count
        //    length = length / 2;



        //    try
        //    {
        //        using (var conn = new OdbcConnection(_configuration))
        //        {
        //            await conn.OpenAsync();

        //            // Query to count total records (without filtering)
        //            string countQuery = @"
        //                            SELECT COUNT(*) 
        //                            FROM (
        //                                SELECT a.pmidno
        //                                FROM S06421A5.PISPROD.pip007 a
        //                                WHERE a.pmstat = 'O'

        //                                UNION

        //                                SELECT a.pmidno
        //                                FROM S06421A5.PISPROD.pip009 a
        //                                WHERE a.pmstat = 'O'
        //                            ) AS total";

        //            using (var countCmd = new OdbcCommand(countQuery, conn))
        //            {
        //                totalRecords = Convert.ToInt32(await countCmd.ExecuteScalarAsync());
        //            }

        //            // Main query with pagination using ROW_NUMBER()
        //            string query = @"
        //                                SELECT * FROM (
        //                                    SELECT 'Active' AS Status, a.pmidno, a.pmlnam, a.pmfnam, a.pmmnam, a.pmides, a.pmbdat, a.pmremk,
        //                                        CASE 
        //                                            WHEN SUBSTR(a.pmicod, 1, 2) <> '00' THEN c.pcdes1
        //                                            WHEN SUBSTR(a.pmicod, 3, 4) IN ('0127', '0129') THEN c.pcdes1
        //                                            ELSE b.pcdes1
        //                                        END AS DeptDiv,
        //                                        ROW_NUMBER() OVER (ORDER BY a.pmidno) AS RowNum
        //                                    FROM S06421A5.PISPROD.pip007 a
        //                                    LEFT OUTER JOIN S06421A5.PISPROD.pip010 b 
        //                                        ON SUBSTR(a.pmicod, 3, 4) = b.pcval AND b.pccdno = '002'
        //                                    LEFT OUTER JOIN S06421A5.PISPROD.pip010 c 
        //                                        ON SUBSTR(a.pmicod, 3, 6) = c.pcval AND c.pccdno = '003'
        //                                    WHERE a.pmstat = 'O'

        //                                    UNION

        //                                    SELECT 'Seceded' AS Status, a.pmidno, a.pmlnam, a.pmfnam, a.pmmnam, a.pmides, a.pmbdat, a.pmremk,
        //                                        CASE 
        //                                            WHEN SUBSTR(a.pmicod, 1, 2) <> '00' THEN c.pcdes1
        //                                            WHEN SUBSTR(a.pmicod, 3, 4) IN ('0127', '0129') THEN c.pcdes1
        //                                            ELSE b.pcdes1
        //                                        END AS DeptDiv,
        //                                        ROW_NUMBER() OVER (ORDER BY a.pmidno) AS RowNum
        //                                    FROM S06421A5.PISPROD.pip009 a
        //                                    LEFT OUTER JOIN S06421A5.PISPROD.pip010 b 
        //                                        ON SUBSTR(a.pmicod, 3, 4) = b.pcval AND b.pccdno = '002'
        //                                    LEFT OUTER JOIN S06421A5.PISPROD.pip010 c 
        //                                        ON SUBSTR(a.pmicod, 3, 6) = c.pcval AND c.pccdno = '003'
        //                                    WHERE a.pmstat = 'O'
        //                                ) AS PaginatedResults
        //                                WHERE RowNum BETWEEN ? AND ?";

        //            using (var cmd = new OdbcCommand(query, conn))
        //            {
        //                cmd.Parameters.AddWithValue("?", start + 1); // This is correct
        //                cmd.Parameters.AddWithValue("?", start + length); // This is also correct

        //                using (var reader = await cmd.ExecuteReaderAsync())
        //                {
        //                    while (await reader.ReadAsync())
        //                    {
        //                        empList.Add(new vmAS400EmployeeList
        //                        {
        //                            IdNumber = reader["pmidno"].ToString().Trim(),
        //                            IdEnc = _encrptionService.Encrypt(_key, reader["pmidno"].ToString().Trim()),
        //                            FullName = $"{reader["pmfnam"].ToString().Trim()} {reader["pmmnam"].ToString().Trim()} {reader["pmlnam"].ToString().Trim()}",
        //                            LastName = reader["pmlnam"].ToString().Trim(),
        //                            FirstName = reader["pmfnam"].ToString().Trim(),
        //                            MiddleName = reader["pmmnam"].ToString().Trim(),
        //                            Position = reader["pmides"].ToString().Trim(),
        //                            BirthDate = ConvertToDateOnly(reader["pmbdat"].ToString().Trim())
        //                        });
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new DatabaseConnectionException("Cannot connect to the AS400 database.", ex);
        //    }

        //    return (empList, totalRecords); // Return the list and total record count
        //}

        public async Task<(List<vmAS400EmployeeList> empList, int totalRecords)> GetEmployeeListAS400(int start, int length, string searchValue)
        {
            var empList = new List<vmAS400EmployeeList>();
            int totalRecords = 0;
            if (!string.IsNullOrEmpty(searchValue) || searchValue.Length > 0)
            {
                searchValue = $"%{searchValue.Trim().ToUpper()}%"; ;
            }
            else
            {
                searchValue = $"%%";
            }

            try
            {
                using (var conn = new OdbcConnection(_configuration))
                {
                    await conn.OpenAsync();

                    // Query to count total records (with search)
                    string countQuery = @"
                SELECT COUNT(*) 
                FROM (
                    SELECT a.pmidno
                    FROM S06421A5.PISPROD.pip007 a
                    WHERE a.pmstat = 'O'
                    AND (a.pmidno LIKE ? OR (a.pmfnam || ' ' || a.pmmnam || ' ' || a.pmlnam) LIKE ?)

                    UNION

                    SELECT a.pmidno
                    FROM S06421A5.PISPROD.pip009 a
                    WHERE a.pmstat = 'O'
                    AND (a.pmidno LIKE ? OR (a.pmfnam || ' ' || a.pmmnam || ' ' || a.pmlnam) LIKE ?)
                ) AS total
            ";

                    using (var countCmd = new OdbcCommand(countQuery, conn))
                    {
                        // Prepare the search value for wildcard searching
                        string searchParam = $"%{searchValue}%";
                        countCmd.Parameters.AddWithValue("?", searchParam);
                        countCmd.Parameters.AddWithValue("?", searchParam);
                        countCmd.Parameters.AddWithValue("?", searchParam);
                        countCmd.Parameters.AddWithValue("?", searchParam);

                        totalRecords = Convert.ToInt32(await countCmd.ExecuteScalarAsync());
                    }

                    // Main query with pagination using a derived table and search
                    string query = @"
                SELECT *
                FROM (
                    SELECT 
                        Status, 
                        pmidno, 
                        pmlnam, 
                        pmfnam, 
                        pmmnam, 
                        pmides, 
                        pmbdat, 
                        pmremk, 
                        DeptDiv,
                        pmhidt,
                        ROW_NUMBER() OVER (ORDER BY pmidno) AS RowNum
                    FROM (
                        SELECT 
                            'Active' AS Status, 
                            a.pmidno, 
                            a.pmlnam, 
                            a.pmfnam, 
                            a.pmmnam, 
                            a.pmides, 
                            a.pmbdat, 
                            a.pmremk,
                            a.pmhidt,
                            CASE 
                                WHEN SUBSTR(a.pmicod, 1, 2) <> '00' THEN c.pcdes1
                                WHEN SUBSTR(a.pmicod, 3, 4) IN ('0127', '0129') THEN c.pcdes1
                                ELSE b.pcdes1
                            END AS DeptDiv
                        FROM S06421A5.PISPROD.pip007 a
                        LEFT OUTER JOIN S06421A5.PISPROD.pip010 b 
                            ON SUBSTR(a.pmicod, 3, 4) = b.pcval AND b.pccdno = '002'
                        LEFT OUTER JOIN S06421A5.PISPROD.pip010 c 
                            ON SUBSTR(a.pmicod, 3, 6) = c.pcval AND c.pccdno = '003'
                        WHERE a.pmstat = 'O'
                        AND (a.pmidno LIKE ? OR (a.pmfnam || ' ' || a.pmmnam || ' ' || a.pmlnam) LIKE ?)

                        UNION ALL

                        SELECT 
                            'Seceded' AS Status, 
                            a.pmidno, 
                            a.pmlnam, 
                            a.pmfnam, 
                            a.pmmnam, 
                            a.pmides, 
                            a.pmbdat, 
                            a.pmremk,
                            a.pmhidt,
                            CASE 
                                WHEN SUBSTR(a.pmicod, 1, 2) <> '00' THEN c.pcdes1
                                WHEN SUBSTR(a.pmicod, 3, 4) IN ('0127', '0129') THEN c.pcdes1
                                ELSE b.pcdes1
                            END AS DeptDiv
                        FROM S06421A5.PISPROD.pip009 a
                        LEFT OUTER JOIN S06421A5.PISPROD.pip010 b 
                            ON SUBSTR(a.pmicod, 3, 4) = b.pcval AND b.pccdno = '002'
                        LEFT OUTER JOIN S06421A5.PISPROD.pip010 c 
                            ON SUBSTR(a.pmicod, 3, 6) = c.pcval AND c.pccdno = '003'
                        WHERE a.pmstat = 'O'
                        AND (a.pmidno LIKE ? OR (a.pmfnam || ' ' || a.pmmnam || ' ' || a.pmlnam) LIKE ?)
                    ) AS CombinedResults
                ) AS ResultWithRowNum
                WHERE RowNum BETWEEN ? AND ?";

                    using (var cmd = new OdbcCommand(query, conn))
                    {
                        // Set parameters for search and pagination
                        cmd.Parameters.AddWithValue("?", $"%{searchValue}%");
                        cmd.Parameters.AddWithValue("?", $"%{searchValue}%");
                        cmd.Parameters.AddWithValue("?", $"%{searchValue}%");
                        cmd.Parameters.AddWithValue("?", $"%{searchValue}%");
                        cmd.Parameters.AddWithValue("?", start + 1); // Start Row Number
                        cmd.Parameters.AddWithValue("?", start + length); // End Row Number

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                empList.Add(new vmAS400EmployeeList
                                {
                                    IdNumber = reader["pmidno"].ToString().Trim(),
                                    IdEnc = _encrptionService.Encrypt(_key, reader["pmidno"].ToString().Trim()),
                                    FullName = $"{reader["pmfnam"].ToString().Trim()} {reader["pmmnam"].ToString().Trim()} {reader["pmlnam"].ToString().Trim()}",
                                    DateHired = ConvertToDateOnly(reader.GetString(reader.GetOrdinal("pmhidt")).ToString().Trim()), //reader["pmhidt,"].ToString().Trim(),
                                    LastName = reader["pmlnam"].ToString().Trim(),
                                    FirstName = reader["pmfnam"].ToString().Trim(),
                                    MiddleName = reader["pmmnam"].ToString().Trim(),
                                    Position = reader["pmides"].ToString().Trim(),
                                    BirthDate = ConvertToDateOnly(reader["pmbdat"].ToString().Trim())

                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new DatabaseConnectionException("Cannot connect to the AS400 database.", ex);
            }

            return (empList, totalRecords);
        }



        public vmEmployeeHealthInfo GetEmployeeInfoUsingIdNumber(string idnumber)
        {
            var empDetail = new vmEmployeeHealthInfo();

            try
            {
                using (var conn = new OdbcConnection(_configuration))
                {
                    conn.Open();

                    // Simplified and formatted query
                    string query = @"
                                SELECT *
                                FROM (
                                    SELECT 
                                        Status, 
                                        pmidno, 
                                        pmlnam, 
                                        pmfnam, 
                                        pmmnam, 
                                        pmides, 
                                        pmbdat, 
                                        pmremk, 
                                        DeptDiv,
                                        pmhidt,
                                        ROW_NUMBER() OVER (ORDER BY pmidno) AS RowNum
                                    FROM (
                                        SELECT 
                                            'Active' AS Status, 
                                            a.pmidno, 
                                            a.pmlnam, 
                                            a.pmfnam, 
                                            a.pmmnam, 
                                            a.pmides, 
                                            a.pmbdat, 
                                            a.pmremk,
                                            a.pmhidt,
                                            CASE 
                                                WHEN SUBSTR(a.pmicod, 1, 2) <> '00' THEN c.pcdes1
                                                WHEN SUBSTR(a.pmicod, 3, 4) IN ('0127', '0129') THEN c.pcdes1
                                                ELSE b.pcdes1
                                            END AS DeptDiv
                                        FROM S06421A5.PISPROD.pip007 a
                                        LEFT OUTER JOIN S06421A5.PISPROD.pip010 b 
                                            ON SUBSTR(a.pmicod, 3, 4) = b.pcval AND b.pccdno = '002'
                                        LEFT OUTER JOIN S06421A5.PISPROD.pip010 c 
                                            ON SUBSTR(a.pmicod, 3, 6) = c.pcval AND c.pccdno = '003'
                                        WHERE a.pmstat = 'O'
                                        AND a.pmidno = ?

                                        UNION ALL

                                        SELECT 
                                            'Seceded' AS Status, 
                                            a.pmidno, 
                                            a.pmlnam, 
                                            a.pmfnam, 
                                            a.pmmnam, 
                                            a.pmides, 
                                            a.pmbdat, 
                                            a.pmremk,
                                            a.pmhidt,
                                            CASE 
                                                WHEN SUBSTR(a.pmicod, 1, 2) <> '00' THEN c.pcdes1
                                                WHEN SUBSTR(a.pmicod, 3, 4) IN ('0127', '0129') THEN c.pcdes1
                                                ELSE b.pcdes1
                                            END AS DeptDiv
                                        FROM S06421A5.PISPROD.pip009 a
                                        LEFT OUTER JOIN S06421A5.PISPROD.pip010 b 
                                            ON SUBSTR(a.pmicod, 3, 4) = b.pcval AND b.pccdno = '002'
                                        LEFT OUTER JOIN S06421A5.PISPROD.pip010 c 
                                            ON SUBSTR(a.pmicod, 3, 6) = c.pcval AND c.pccdno = '003'
                                        WHERE a.pmstat = 'O'
                                        AND a.pmidno = ?
                                    ) AS CombinedResults
                                ) AS ResultWithRowNum
                                WHERE pmidno = ?";

                    using (var cmd = new OdbcCommand(query, conn))
                    {
                        // Set parameters for search and pagination
                        cmd.Parameters.AddWithValue("?", $"{idnumber}");
                        cmd.Parameters.AddWithValue("?", $"{idnumber}");
                        cmd.Parameters.AddWithValue("?", $"{idnumber}");


                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                empDetail = (new vmEmployeeHealthInfo
                                {
                                    EmployeeDetailAS400 = new vmAS400EmployeeList
                                    {
                                        IdNumber = reader["pmidno"].ToString().Trim(),
                                        IdEnc = _encrptionService.Encrypt(_key, reader["pmidno"].ToString().Trim()),
                                        FullName = $"{reader["pmfnam"].ToString().Trim()} {reader["pmmnam"].ToString().Trim()} {reader["pmlnam"].ToString().Trim()}",
                                        LastName = reader["pmlnam"].ToString().Trim(),
                                        FirstName = reader["pmfnam"].ToString().Trim(),
                                        MiddleName = reader["pmmnam"].ToString().Trim(),
                                        Position = reader["pmides"].ToString().Trim(),
                                        BirthDate = ConvertToDateOnly(reader["pmbdat"].ToString().Trim()),
                                        DateHired = ConvertToDateOnly(reader.GetString(reader.GetOrdinal("pmhidt")).ToString().Trim()), //reader["pmhidt,"].ToString().Trim(),
                                    },
                                });
                            }
                        }
                    }
                }


                //get the other details using other repository
            }
            catch (Exception ex)
            {
                throw new DatabaseConnectionException("Cannot connect to the AS400 database.", ex);
            }

            return empDetail;
        }





        public async Task<List<vmBranch>> GetAllBranches()
        {
            var branchRec = new List<vmBranch>();

            try
            {
                using (var conn = new OdbcConnection(_configuration))
                {
                    await conn.OpenAsync();

                    using (var cmd = new OdbcCommand(
                        " SELECT " +
                        " substr(a.pmicod, 1, 2) as branchCode," +
                        " a.PMFIL as branchDesc" +
                        " FROM" +
                        " S06421A5.PISPROD.pip007 a" +
                        " LEFT JOIN" +
                        " S06421A5.PISPROD.pip010 b" +
                        " ON" +
                        " b.pcval = substr(a.pmicod, 1, 2)" +
                        " AND b.pccdno = '001'" +
                        " GROUP BY" +
                        " substr(a.pmicod, 1, 2)," +
                        " a.PMFIL ", conn))
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                branchRec.Add(new vmBranch
                                {
                                    BranchCodeEncrypt = _encrptionService.EncryptConstant(_key, reader.GetString(reader.GetOrdinal("branchCode")).Trim()),
                                    BranchCode = reader.GetString(reader.GetOrdinal("branchCode")).Trim(),
                                    BranchDescription = reader.GetString(reader.GetOrdinal("branchDesc")).Trim(),
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new DatabaseConnectionException("Cannot connect to the AS400 database.", ex);
            }



            return branchRec;
        }

        // Method to convert integer to date
        private DateOnly ConvertToDateOnly(string birthDateString)
        {
            if (string.IsNullOrEmpty(birthDateString) || birthDateString == "0")
            {
                return new DateOnly(1900, 1, 1); // Return default date
            }

            if (birthDateString.Length == 8) // Expecting YYYYMMDD
            {
                if (int.TryParse(birthDateString.Substring(0, 4), out int year) &&
                    int.TryParse(birthDateString.Substring(4, 2), out int month) &&
                    int.TryParse(birthDateString.Substring(6, 2), out int day))
                {
                    return new DateOnly(year, month, day);
                }
            }

            return new DateOnly(1900, 1, 1); // Fallback if format is unexpected
        }
    }
}
